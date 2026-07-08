using EmissorNotaFiscal.Domain.Interfaces;
using EmissorNotaFiscal.Domain.Models.Automation;
using EmissorNotaFiscal.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace EmissorNotaFiscal.Infrastructure.Automation;

public sealed class ContractBasedAutomationEngine : INfeAutomationService
{
    private const string HeadlessConfigurationKey = "Automation:Headless";
    private const string DownloadsDirectoryConfigurationKey = "Automation:DownloadsDirectory";
    private static readonly TimeSpan OptionalActionTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan AssistedModePollInterval = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan AssistedModeProgressLogInterval = TimeSpan.FromSeconds(30);
    private const string DiagnosticsDirectoryName = "diagnostics";
    private static readonly string[] ChallengeDetectionSelectors =
    [
        "#mcaptcha__token-label",
        "img[src*='captcha' i]",
        "img[src*='mcaptcha' i]",
        "audio[src*='captcha' i]",
        "audio[src*='mcaptcha' i]",
        "input[id*='captcha' i]",
        "input[name*='captcha' i]",
        "input[id*='mcaptcha' i]",
        "input[name*='mcaptcha' i]"
    ];

    private readonly AssistedAutomationOptions _assistedAutomationOptions;
    private readonly CaptchaSolverOptions _captchaSolverOptions;
    private readonly ICaptchaSolverService? _captchaSolverService;
    private readonly IConfiguration? _configuration;
    private readonly ILogger<ContractBasedAutomationEngine> _logger;

    public ContractBasedAutomationEngine(
        ILogger<ContractBasedAutomationEngine> logger,
        IConfiguration? configuration = null,
        IOptions<AssistedAutomationOptions>? assistedAutomationOptions = null,
        ICaptchaSolverService? captchaSolverService = null,
        IOptions<CaptchaSolverOptions>? captchaSolverOptions = null)
    {
        _logger = logger;
        _configuration = configuration;
        _assistedAutomationOptions = assistedAutomationOptions?.Value ?? new AssistedAutomationOptions();
        _captchaSolverService = captchaSolverService;
        _captchaSolverOptions = captchaSolverOptions?.Value ?? new CaptchaSolverOptions();
    }

    public async Task<string> ExecutarFluxoContratoAsync(
        FluxoAutomacaoContrato contrato,
        Dictionary<string, string> dicionarioDadosReais)
    {
        ArgumentNullException.ThrowIfNull(contrato);
        ArgumentNullException.ThrowIfNull(dicionarioDadosReais);

        string downloadsDirectory = ResolveDownloadsDirectory();
        Directory.CreateDirectory(downloadsDirectory);

        TaskCompletionSource<string> downloadPathSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

        using IPlaywright playwright = await Playwright.CreateAsync();
        await using IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = ResolveHeadlessMode()
        });
        await using IBrowserContext browserContext = await browser.NewContextAsync(new BrowserNewContextOptions
        {
            AcceptDownloads = true
        });

        IPage page = await browserContext.NewPageAsync();
        RegisterPageDiagnostics(page);
        RegisterDownloadCapture(page, downloadPathSource, downloadsDirectory);
        await NavegarParaUrlInicialAsync(page, contrato);

        List<EtapaExecucao> etapasOrdenadas = contrato.Etapas.OrderBy(etapa => etapa.Ordem).ToList();

        foreach (EtapaExecucao etapa in etapasOrdenadas)
        {
            await ExecutarEtapaAsync(page, contrato, etapa, dicionarioDadosReais);
        }

        return await ResolveDownloadedPdfPathAsync(downloadPathSource.Task);
    }

    private async Task ExecutarEtapaAsync(
        IPage page,
        FluxoAutomacaoContrato contrato,
        EtapaExecucao etapa,
        IReadOnlyDictionary<string, string> dicionarioDadosReais)
    {
        if (etapa.Opcional)
        {
            bool etapaExecutada = false;

            foreach (AcaoPasso acao in etapa.Acoes)
            {
                try
                {
                    etapaExecutada |= await ExecutarPassoAsync(
                        page,
                        contrato,
                        etapa,
                        acao,
                        dicionarioDadosReais,
                        passoOpcional: true);
                }
                catch (AutomationExecutionException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Falha ao executar a etapa opcional {OrdemEtapa} ({NomeEtapa}) no passo '{DescricaoAcao}'.",
                        etapa.Ordem,
                        etapa.NomeEtapa,
                        acao.Descricao);

                    throw new AutomationExecutionException(
                        etapa.Ordem,
                        etapa.NomeEtapa,
                        acao.Descricao,
                        "Falha durante a execucao do passo opcional de automacao.",
                        ex);
                }
            }

            _logger.LogInformation(
                etapaExecutada
                    ? "Etapa opcional {OrdemEtapa} ({NomeEtapa}) foi executada."
                    : "Etapa opcional {OrdemEtapa} ({NomeEtapa}) foi ignorada porque o portal nao exibiu o seletor esperado.",
                etapa.Ordem,
                etapa.NomeEtapa);

            return;
        }

        for (int acaoIndex = 0; acaoIndex < etapa.Acoes.Count; acaoIndex++)
        {
            AcaoPasso acao = etapa.Acoes[acaoIndex];

            try
            {
                bool passoExecutado = await ExecutarPassoAsync(page, contrato, etapa, acao, dicionarioDadosReais);

                if (passoExecutado)
                {
                    await AguardarModoAssistidoSeNecessarioAsync(page, contrato, etapa, acaoIndex);
                }
            }
            catch (AutomationExecutionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Falha ao executar a etapa {OrdemEtapa} ({NomeEtapa}) no passo '{DescricaoAcao}'.",
                    etapa.Ordem,
                    etapa.NomeEtapa,
                    acao.Descricao);

                throw new AutomationExecutionException(
                    etapa.Ordem,
                    etapa.NomeEtapa,
                    acao.Descricao,
                    "Falha durante a execucao do passo de automacao.",
                    ex);
            }
        }
    }

    private async Task NavegarParaUrlInicialAsync(IPage page, FluxoAutomacaoContrato contrato)
    {
        string urlInicial = ValidarUrlInicial(contrato.UrlInicial);

        _logger.LogInformation("Iniciando bootstrap de navegacao na UrlInicial {UrlInicial}.", urlInicial);

        try
        {
            await page.GotoAsync(urlInicial);
            _logger.LogInformation("Bootstrap de navegacao concluido para {UrlInicial}.", urlInicial);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Falha ao abrir a UrlInicial {UrlInicial} antes do processamento das etapas.",
                urlInicial);

            throw new AutomationExecutionException(
                ordemEtapa: 0,
                nomeEtapa: "Bootstrap de UrlInicial",
                descricaoAcao: "Navegacao inicial automatica",
                "Falha ao navegar para a UrlInicial do contrato.",
                ex);
        }
    }

    private async Task<bool> ExecutarPassoAsync(
        IPage page,
        FluxoAutomacaoContrato contrato,
        EtapaExecucao etapa,
        AcaoPasso acao,
        IReadOnlyDictionary<string, string> dicionarioDadosReais,
        bool passoOpcional = false)
    {
        switch (acao.PlaywrightAcao)
        {
            case TipoAcao.Navegar:
                await page.GotoAsync(ResolveNavigationTarget(contrato, acao, dicionarioDadosReais));
                return true;

            case TipoAcao.PreencherTexto:
                await PreencherTextoAsync(page, etapa, acao, dicionarioDadosReais);
                return true;

            case TipoAcao.ClicarBotao:
                return await ClickAsync(page, etapa, acao, passoOpcional || acao.Opcional);

            case TipoAcao.ClicarSeExistir:
                return await ClickAsync(page, etapa, acao, true, aguardarNavegacao: true);

            case TipoAcao.DispararBlur:
                await DispararBlurAsync(page, acao);
                return true;

            case TipoAcao.AguardarCarregamento:
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                return true;

            case TipoAcao.TratarDialogos:
                RegistrarTratamentoDialogos(page);
                return true;

            default:
                throw new AutomationExecutionException(
                    etapa.Ordem,
                    etapa.NomeEtapa,
                    acao.Descricao,
                    $"O tipo de acao '{acao.PlaywrightAcao}' nao e suportado pelo motor de automacao.");
        }
    }

    private static string ResolveNavigationTarget(
        FluxoAutomacaoContrato contrato,
        AcaoPasso acao,
        IReadOnlyDictionary<string, string> dicionarioDadosReais)
    {
        if (!string.IsNullOrWhiteSpace(acao.ValorDinamicoChave))
        {
            return GetRequiredRuntimeValue(acao.ValorDinamicoChave, acao.Descricao, dicionarioDadosReais);
        }

        if (!string.IsNullOrWhiteSpace(acao.SeletorHtml))
        {
            return acao.SeletorHtml;
        }

        if (!string.IsNullOrWhiteSpace(contrato.UrlInicial))
        {
            return contrato.UrlInicial;
        }

        throw new InvalidOperationException(
            $"O passo '{acao.Descricao}' nao possui destino de navegacao configurado.");
    }

    private static string ValidarUrlInicial(string urlInicial)
    {
        if (string.IsNullOrWhiteSpace(urlInicial))
        {
            throw new InvalidOperationException(
                "O contrato de automacao exige uma UrlInicial valida para iniciar a execucao.");
        }

        if (!Uri.TryCreate(urlInicial, UriKind.Absolute, out Uri? uri))
        {
            throw new InvalidOperationException(
                $"A UrlInicial '{urlInicial}' nao e um endereco absoluto valido.");
        }

        return uri.ToString();
    }

    private static async Task PreencherTextoAsync(
        IPage page,
        EtapaExecucao etapa,
        AcaoPasso acao,
        IReadOnlyDictionary<string, string> dicionarioDadosReais)
    {
        if (string.IsNullOrWhiteSpace(acao.ValorDinamicoChave))
        {
            throw new AutomationExecutionException(
                etapa.Ordem,
                etapa.NomeEtapa,
                acao.Descricao,
                "O passo de preenchimento exige uma chave de valor dinamico.");
        }

        if (IsPmspAuthUrl(page.Url) && (acao.SeletorHtml == "#cpfCnpj" || acao.SeletorHtml == "#password"))
        {
            await GarantirSanfonaSenhaWebExpandidaAsync(page);
        }

        string valor = GetRequiredRuntimeValue(acao.ValorDinamicoChave, acao.Descricao, dicionarioDadosReais);
        ILocator locator = await LocalizarElementoVisivelAsync(page, acao.SeletorHtml);
        await locator.FillAsync(valor);
    }

    private static async Task GarantirSanfonaSenhaWebExpandidaAsync(IPage page)
    {
        try
        {
            if (await page.Locator("#cpfCnpj").IsVisibleAsync())
            {
                return;
            }

            ILocator accordionButton = page.Locator("button.login-accordion-header:has-text(\"Senha Web\"), button:has-text(\"Senha Web\")");
            if (await accordionButton.IsVisibleAsync())
            {
                string? classAttr = await accordionButton.GetAttributeAsync("class");
                string? ariaExpanded = await accordionButton.GetAttributeAsync("aria-expanded");

                bool isCollapsed = (classAttr != null && classAttr.Contains("collapsed", StringComparison.OrdinalIgnoreCase))
                    || (ariaExpanded != null && ariaExpanded.Equals("false", StringComparison.OrdinalIgnoreCase));

                if (isCollapsed)
                {
                    Console.WriteLine("[Engine] Detectada sanfona de Senha Web recolhida. Expandindo...");
                    await accordionButton.ClickAsync();
                    
                    // Aguarda o elemento ficar visível na transição
                    await page.Locator("#cpfCnpj").WaitForAsync(new LocatorWaitForOptions
                    {
                        State = WaitForSelectorState.Visible,
                        Timeout = 3000
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Engine] Falha ao tentar garantir sanfona Senha Web expandida: {ex.Message}");
        }
    }

    private async Task<bool> ClickAsync(IPage page, EtapaExecucao etapa, AcaoPasso acao, bool opcional)
        => await ClickAsync(page, etapa, acao, opcional, aguardarNavegacao: false);

    private async Task<bool> ClickAsync(
        IPage page,
        EtapaExecucao etapa,
        AcaoPasso acao,
        bool opcional,
        bool aguardarNavegacao)
    {
        if (opcional)
        {
            bool seletorEncontrado = await TentarLocalizarElementoVisivelAsync(
                page,
                acao.SeletorHtml,
                OptionalActionTimeout);

            if (!seletorEncontrado)
            {
                _logger.LogInformation(
                    "Etapa opcional {OrdemEtapa} ({NomeEtapa}) ignorou o passo '{DescricaoAcao}' porque o seletor '{SeletorHtml}' nao foi encontrado.",
                    etapa.Ordem,
                    etapa.NomeEtapa,
                    acao.Descricao,
                    acao.SeletorHtml);

                return false;
            }
        }

        ILocator locator = await LocalizarElementoVisivelAsync(page, acao.SeletorHtml);

        if (aguardarNavegacao)
        {
            var urlTask = page.WaitForURLAsync("**", new PageWaitForURLOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            await locator.ClickAsync();

            try
            {
                await urlTask;
                _logger.LogInformation(
                    "Login Unico concluiu a navegacao. URL final atual: {FinalUrl}",
                    page.Url);

                if (page.Url.StartsWith("chrome-error://", StringComparison.OrdinalIgnoreCase))
                {
                    await CapturePageDiagnosticsAsync(page, "login-unico-chrome-error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "A navegacao apos o Login Unico nao foi concluida. URL atual: {CurrentUrl}",
                    page.Url);

                throw;
            }
        }
        else
        {
            await locator.ClickAsync();
        }

        return true;
    }

    private async Task AguardarModoAssistidoSeNecessarioAsync(
        IPage page,
        FluxoAutomacaoContrato contrato,
        EtapaExecucao etapa,
        int acaoIndex)
    {
        if (!IsPmspAuthUrl(page.Url) || !await IsChallengeVisibleAsync(page))
        {
            return;
        }

        // 1. Tentar resolver o captcha automaticamente via Vision AI
        if (_captchaSolverOptions.Enabled && _captchaSolverService != null)
        {
            bool resolvido = await TentarResolverCaptchaAutomaticamenteAsync(page, contrato, etapa, acaoIndex);
            if (resolvido)
            {
                return;
            }
        }

        // 2. Se falhar ou estiver desativado, desvia para o Modo Assistido se ativo
        if (!_assistedAutomationOptions.Enabled)
        {
            _logger.LogError("Desafio de Captcha detectado e a resolução automática falhou ou está desativada, e o Modo Assistido está desabilitado.");
            await CapturePageDiagnosticsAsync(page, "captcha-solver-failed-error");
            throw new AutomationExecutionException(
                etapa.Ordem,
                etapa.NomeEtapa,
                etapa.Acoes[acaoIndex].Descricao,
                "Desafio de captcha detectado no portal. Resolução automática e Modo Assistido indisponíveis.");
        }

        TimeSpan timeout = TimeSpan.FromMinutes(_assistedAutomationOptions.HumanInterventionTimeoutMinutes);
        DateTimeOffset deadline = DateTimeOffset.UtcNow.Add(timeout);
        DateTimeOffset nextProgressLog = DateTimeOffset.UtcNow.Add(AssistedModeProgressLogInterval);

        _logger.LogWarning(
            "Modo assistido detectou challenge/caption em {CurrentUrl}. Aguardando intervencao humana por ate {TimeoutMinutes} minuto(s).",
            page.Url,
            _assistedAutomationOptions.HumanInterventionTimeoutMinutes);

        await CapturePageDiagnosticsAsync(page, "assisted-challenge-detected");

        while (DateTimeOffset.UtcNow < deadline)
        {
            await Task.Delay(AssistedModePollInterval);

            if (await PossuiEstadoValidoDeContinuidadeAsync(page, contrato, etapa, acaoIndex))
            {
                _logger.LogInformation(
                    "Modo assistido retomou o fluxo automaticamente apos a intervencao humana. URL atual: {CurrentUrl}",
                    page.Url);

                return;
            }

            if (DateTimeOffset.UtcNow >= nextProgressLog)
            {
                _logger.LogInformation(
                    "Modo assistido ainda aguarda resolucao humana do challenge/caption. URL atual: {CurrentUrl}",
                    page.Url);

                nextProgressLog = DateTimeOffset.UtcNow.Add(AssistedModeProgressLogInterval);
            }
        }

        await CapturePageDiagnosticsAsync(page, "assisted-challenge-timeout");

        throw new AutomationExecutionException(
            etapa.Ordem,
            etapa.NomeEtapa,
            etapa.Acoes[acaoIndex].Descricao,
            $"Modo assistido expirou apos {_assistedAutomationOptions.HumanInterventionTimeoutMinutes} minuto(s) aguardando resolucao humana do challenge/caption.");
    }

    private async Task<bool> TentarResolverCaptchaAutomaticamenteAsync(
        IPage page,
        FluxoAutomacaoContrato contrato,
        EtapaExecucao etapa,
        int acaoIndex)
    {
        _logger.LogInformation("Iniciando fluxo de resolução automática de captcha via Vision AI.");

        for (int tentativa = 1; tentativa <= _captchaSolverOptions.MaxRetries; tentativa++)
        {
            _logger.LogInformation("Tentativa {Tentativa} de {MaxRetries} de resolução automática do captcha.", tentativa, _captchaSolverOptions.MaxRetries);

            try
            {
                // 1. Localizar o elemento da imagem do Captcha
                ILocator captchaImgLocator = page.Locator(_captchaSolverOptions.Selectors.CaptchaImage);
                await captchaImgLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });

                // 2. Tirar screenshot apenas do elemento da imagem
                byte[] screenshotBytes = await captchaImgLocator.ScreenshotAsync(new LocatorScreenshotOptions
                {
                    Type = ScreenshotType.Png
                });

                string base64Image = Convert.ToBase64String(screenshotBytes);

                // 3. Enviar para a API de Vision
                string captchaResposta = await _captchaSolverService!.SolveAsync(base64Image);

                if (string.IsNullOrWhiteSpace(captchaResposta))
                {
                    _logger.LogWarning("O resolvedor de captcha retornou uma resposta vazia na tentativa {Tentativa}.", tentativa);
                    await RecarregarCaptchaSeNecessarioAsync(page);
                    continue;
                }

                // 4. Preencher a resposta no campo
                ILocator inputLocator = page.Locator(_captchaSolverOptions.Selectors.InputResponse);
                await inputLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
                await inputLocator.FillAsync(captchaResposta);

                // 5. Clicar no botão de enviar
                ILocator submitLocator = page.Locator(_captchaSolverOptions.Selectors.SubmitButton);
                await submitLocator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible, Timeout = 5000 });
                
                // Executa o clique e aguarda o estado de rede ocioso
                await submitLocator.ClickAsync();
                
                try
                {
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new PageWaitForLoadStateOptions { Timeout = 5000 });
                }
                catch (TimeoutException)
                {
                    _logger.LogWarning("Timeout aguardando estado NetworkIdle após envio do captcha.");
                }

                // 6. Verificar se ainda há captcha ou se avançou
                await Task.Delay(1500);

                if (await PossuiEstadoValidoDeContinuidadeAsync(page, contrato, etapa, acaoIndex))
                {
                    _logger.LogInformation("Captcha resolvido com sucesso! O fluxo prosseguirá.");
                    return true;
                }

                _logger.LogWarning("Captcha incorreto na tentativa {Tentativa}. O formulário foi recarregado.", tentativa);
                
                if (tentativa < _captchaSolverOptions.MaxRetries)
                {
                    await RecarregarCaptchaSeNecessarioAsync(page);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha na tentativa {Tentativa} de resolução automática do captcha.", tentativa);
                if (tentativa < _captchaSolverOptions.MaxRetries)
                {
                    await RecarregarCaptchaSeNecessarioAsync(page);
                }
            }
        }

        _logger.LogWarning("Todas as {MaxRetries} tentativas de resolução automática falharam.", _captchaSolverOptions.MaxRetries);
        return false;
    }

    private async Task RecarregarCaptchaSeNecessarioAsync(IPage page)
    {
        if (string.IsNullOrWhiteSpace(_captchaSolverOptions.Selectors.ReloadButton))
        {
            return;
        }

        try
        {
            ILocator reloadLocator = page.Locator(_captchaSolverOptions.Selectors.ReloadButton);
            if (await reloadLocator.IsVisibleAsync())
            {
                _logger.LogInformation("Disparando recarga de captcha via botão de reload.");
                await reloadLocator.ClickAsync();
                await Task.Delay(1000); // Aguarda a imagem atualizar
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Não foi possível clicar no botão de recarga do captcha.");
        }
    }

    private async Task<bool> PossuiEstadoValidoDeContinuidadeAsync(
        IPage page,
        FluxoAutomacaoContrato contrato,
        EtapaExecucao etapa,
        int acaoIndex)
    {
        if (await IsChallengeVisibleAsync(page))
        {
            return false;
        }

        string[] continuationSelectors = ObterSeletoresDeContinuidade(contrato, etapa.Ordem, acaoIndex);

        if (continuationSelectors.Length > 0 && await ExisteQualquerSeletorVisivelAsync(page, continuationSelectors))
        {
            return true;
        }

        return !IsPmspAuthUrl(page.Url);
    }

    private static string[] ObterSeletoresDeContinuidade(
        FluxoAutomacaoContrato contrato,
        int ordemEtapaAtual,
        int acaoIndexAtual)
    {
        return contrato.Etapas
            .OrderBy(etapa => etapa.Ordem)
            .Where(etapa => etapa.Ordem >= ordemEtapaAtual)
            .SelectMany(etapa => etapa.Ordem == ordemEtapaAtual
                ? etapa.Acoes.Skip(acaoIndexAtual + 1)
                : etapa.Acoes)
            .Select(acao => acao.SeletorHtml)
            .Where(seletor => !string.IsNullOrWhiteSpace(seletor))
            .Distinct(StringComparer.Ordinal)
            .Take(5)
            .ToArray()!;
    }

    private async Task<bool> IsChallengeVisibleAsync(IPage page)
    {
        if (!IsPmspAuthUrl(page.Url))
        {
            return false;
        }

        return await ExisteQualquerSeletorVisivelAsync(page, ChallengeDetectionSelectors);
    }

    private static bool IsPmspAuthUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri? uri)
            && uri.Host.Contains("pmspauth.prefeitura.sp.gov.br", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<bool> ExisteQualquerSeletorVisivelAsync(IPage page, IEnumerable<string> seletores)
    {
        foreach (string seletor in seletores)
        {
            if (string.IsNullOrWhiteSpace(seletor))
            {
                continue;
            }

            ILocator locator = page.Locator(seletor);

            try
            {
                if (await locator.IsVisibleAsync())
                {
                    return true;
                }
            }
            catch (PlaywrightException)
            {
                // Invalid or unsupported selectors should not break assisted-mode checks.
            }
        }

        return false;
    }

    private void RegisterPageDiagnostics(IPage page)
    {
        page.RequestFailed += (_, request) =>
        {
            _logger.LogWarning(
                "Request do navegador falhou. Metodo: {Method}. URL: {Url}. Erro: {FailureText}",
                request.Method,
                request.Url,
                request.Failure);
        };

        page.PageError += (_, message) =>
        {
            _logger.LogWarning("Erro de pagina capturado pelo navegador: {PageError}", message);
        };
    }

    private async Task CapturePageDiagnosticsAsync(IPage page, string artifactPrefix)
    {
        try
        {
            string diagnosticsDirectory = Path.Combine(ResolveDownloadsDirectory(), DiagnosticsDirectoryName);
            Directory.CreateDirectory(diagnosticsDirectory);

            string stamp = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmssfff");
            string screenshotPath = Path.Combine(diagnosticsDirectory, $"{stamp}-{artifactPrefix}.png");
            string htmlPath = Path.Combine(diagnosticsDirectory, $"{stamp}-{artifactPrefix}.html");
            string metadataPath = Path.Combine(diagnosticsDirectory, $"{stamp}-{artifactPrefix}.txt");

            await page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });

            string html = await page.ContentAsync();
            await File.WriteAllTextAsync(htmlPath, html);

            string title = await page.TitleAsync();
            string metadata = string.Join(
                Environment.NewLine,
                [
                    $"Url={page.Url}",
                    $"Title={title}",
                    $"CapturedAtUtc={DateTimeOffset.UtcNow:O}"
                ]);

            await File.WriteAllTextAsync(metadataPath, metadata);

            _logger.LogWarning(
                "Artefatos de diagnostico salvos em {DiagnosticsDirectory} para a pagina {CurrentUrl}.",
                diagnosticsDirectory,
                page.Url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao capturar artefatos de diagnostico da pagina.");
        }
    }

    private static async Task DispararBlurAsync(IPage page, AcaoPasso acao)
    {
        string seletor = GetRequiredSelector(acao);

        await LocalizarElementoVisivelAsync(page, seletor);
        await page.EvaluateAsync(
            @"(selector) => {
                const element = document.querySelector(selector);
                if (!element) {
                    throw new Error(`Elemento não encontrado para blur: ${selector}`);
                }

                element.blur();
            }",
            seletor);
    }

    private static async Task<ILocator> LocalizarElementoVisivelAsync(IPage page, string seletor)
    {
        if (string.IsNullOrWhiteSpace(seletor))
        {
            throw new InvalidOperationException("O passo de automacao exige um seletor HTML valido.");
        }

        ILocator locator = page.Locator(seletor);
        await locator.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible
        });

        return locator;
    }

    private static async Task<bool> TentarLocalizarElementoVisivelAsync(
        IPage page,
        string seletor,
        TimeSpan timeout)
    {
        if (string.IsNullOrWhiteSpace(seletor))
        {
            throw new InvalidOperationException("O passo de automacao exige um seletor HTML valido.");
        }

        ILocator locator = page.Locator(seletor);

        try
        {
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = (float)timeout.TotalMilliseconds
            });

            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    private void RegistrarTratamentoDialogos(IPage page)
    {
        page.Dialog -= HandleDialogAsync;
        page.Dialog += HandleDialogAsync;
    }

    private async void HandleDialogAsync(object? sender, IDialog dialog)
    {
        try
        {
            _logger.LogInformation(
                "Dialogo automatico capturado. Tipo: {DialogType}. Mensagem: {DialogMessage}",
                dialog.Type,
                dialog.Message);

            await dialog.AcceptAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao tratar automaticamente um dialogo do navegador.");
        }
    }

    private void RegisterDownloadCapture(
        IPage page,
        TaskCompletionSource<string> downloadPathSource,
        string downloadsDirectory)
    {
        page.Download += async (_, download) =>
        {
            try
            {
                string fileName = download.SuggestedFilename;

                if (!fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation(
                        "Download ignorado por nao ser PDF. Nome sugerido: {SuggestedFilename}",
                        fileName);
                    return;
                }

                string fullPath = BuildUniqueDownloadPath(downloadsDirectory, fileName);
                await download.SaveAsAsync(fullPath);
                downloadPathSource.TrySetResult(fullPath);
            }
            catch (Exception ex)
            {
                downloadPathSource.TrySetException(
                    new InvalidOperationException("Falha ao capturar o arquivo PDF baixado.", ex));
            }
        };
    }

    private static string BuildUniqueDownloadPath(string directory, string fileName)
    {
        string safeFileName = Path.GetFileName(fileName);
        string prefix = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmssfff");
        return Path.Combine(directory, $"{prefix}-{safeFileName}");
    }

    private async Task<string> ResolveDownloadedPdfPathAsync(Task<string> downloadPathTask)
    {
        Task completedTask = await Task.WhenAny(downloadPathTask, Task.Delay(TimeSpan.FromSeconds(30)));

        if (completedTask != downloadPathTask)
        {
            throw new InvalidOperationException(
                "A automacao foi concluida sem produzir um caminho de PDF baixado.");
        }

        string downloadPath = await downloadPathTask;

        if (string.IsNullOrWhiteSpace(downloadPath) || !File.Exists(downloadPath))
        {
            throw new InvalidOperationException(
                "A automacao informou um caminho de PDF invalido ou inexistente.");
        }

        return downloadPath;
    }

    private bool ResolveHeadlessMode()
    {
        if (_assistedAutomationOptions.Enabled)
        {
            _logger.LogInformation(
                "Modo assistido habilitado. O navegador sera executado em modo visivel independentemente de {HeadlessConfigurationKey}.",
                HeadlessConfigurationKey);

            return false;
        }

        return _configuration?.GetValue<bool?>(HeadlessConfigurationKey) ?? true;
    }

    private string ResolveDownloadsDirectory()
    {
        return _configuration?.GetValue<string>(DownloadsDirectoryConfigurationKey)
            ?? Path.Combine(AppContext.BaseDirectory, "downloads");
    }

    private static string GetRequiredRuntimeValue(
        string chave,
        string descricaoAcao,
        IReadOnlyDictionary<string, string> dicionarioDadosReais)
    {
        if (!dicionarioDadosReais.TryGetValue(chave, out string? valor) || string.IsNullOrWhiteSpace(valor))
        {
            throw new InvalidOperationException(
                $"O valor dinamico '{chave}' exigido pelo passo '{descricaoAcao}' nao foi informado.");
        }

        return valor;
    }

    private static string GetRequiredSelector(AcaoPasso acao)
    {
        if (string.IsNullOrWhiteSpace(acao.SeletorHtml))
        {
            throw new InvalidOperationException(
                $"O passo '{acao.Descricao}' exige um seletor HTML valido.");
        }

        return acao.SeletorHtml;
    }
}

public sealed class AutomationExecutionException : Exception
{
    public AutomationExecutionException(
        int ordemEtapa,
        string nomeEtapa,
        string descricaoAcao,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        OrdemEtapa = ordemEtapa;
        NomeEtapa = nomeEtapa;
        DescricaoAcao = descricaoAcao;
    }

    public int OrdemEtapa { get; }

    public string NomeEtapa { get; }

    public string DescricaoAcao { get; }
}
