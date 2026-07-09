using EmissorNotaFiscal.Domain.Interfaces;
using EmissorNotaFiscal.Domain.Models.Automation;
using EmissorNotaFiscal.Domain.Models.Faturamento;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EmissorNotaFiscal.Application;

public sealed class FaturamentoOrchestrator
{
    private const string ReceitaFileName = "receita_paulistana.json";
    private const string ConfigNotasFileName = "config_notas_v2.json";
    private const string SenhaWebConfigurationKey = "Automation:SenhaWeb";

    private readonly IConfigRepository _configRepository;
    private readonly INfeAutomationService _automationService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FaturamentoOrchestrator> _logger;

    public FaturamentoOrchestrator(
        IConfigRepository configRepository,
        INfeAutomationService automationService,
        IConfiguration configuration,
        ILogger<FaturamentoOrchestrator> logger)
    {
        _configRepository = configRepository;
        _automationService = automationService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "FaturamentoOrchestrator invoked. JSON-driven automation execution is starting.");

        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            string receitaPath = ResolveRootFilePath(ReceitaFileName);
            string configNotasPath = ResolveRootFilePath(ConfigNotasFileName);

            FluxoAutomacaoContrato contrato =
                await _configRepository.ObterReceitaAutomacaoAsync(receitaPath);
            ConfigFaturamento configuracaoFaturamento =
                await _configRepository.ObterConfiguracaoFaturamentoAsync(configNotasPath);

            ValidarContratoCarregado(contrato, receitaPath);

            ItemNota itemSelecionado = SelecionarItemTeste(configuracaoFaturamento);
            Dictionary<string, string> dicionarioDadosReais =
                ConstruirDicionarioDados(configuracaoFaturamento, itemSelecionado);

            _logger.LogInformation(
                "Selected billing record {Cliente} ({CnpjCliente}) for JSON-driven automation test path.",
                itemSelecionado.RazaoSocialPlaceholder,
                itemSelecionado.CnpjCliente);

            string pdfPath = await _automationService.ExecutarFluxoContratoAsync(contrato, dicionarioDadosReais);

            _logger.LogInformation(
                "JSON-driven automation completed successfully and returned PDF path {PdfPath}.",
                pdfPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "JSON-driven orchestration failed while loading inputs or invoking the automation service.");

            throw;
        }
    }

    private string ResolveRootFilePath(string fileName)
    {
        string currentDirectory = AppContext.BaseDirectory;

        for (DirectoryInfo? directory = new(currentDirectory); directory is not null; directory = directory.Parent)
        {
            string candidate = Path.Combine(directory.FullName, fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new FileNotFoundException(
            $"Nao foi possivel localizar o arquivo obrigatorio '{fileName}' a partir do diretorio de execucao '{currentDirectory}'.",
            fileName);
    }

    private static ItemNota SelecionarItemTeste(ConfigFaturamento configuracaoFaturamento)
    {
        ItemNota? itemSelecionado = configuracaoFaturamento.AgendamentoNotas.FirstOrDefault();

        return itemSelecionado ?? throw new InvalidOperationException(
            "Nenhum item de agendamento de notas foi encontrado para a execucao de teste.");
    }

    private Dictionary<string, string> ConstruirDicionarioDados(
        ConfigFaturamento configuracaoFaturamento,
        ItemNota itemSelecionado)
    {
        string? senhaWeb = _configuration[SenhaWebConfigurationKey];

        if (string.IsNullOrWhiteSpace(senhaWeb))
        {
            throw new InvalidOperationException(
                $"A configuracao obrigatoria '{SenhaWebConfigurationKey}' nao foi informada para a execucao da automacao.");
        }

        DateTime hoje = DateTime.Now;

        Dictionary<string, string> dados = new(StringComparer.OrdinalIgnoreCase)
        {
            ["CnpjPrestador"] = configuracaoFaturamento.ConfiguracoesEmissor.CnpjPrestador,
            ["SenhaWeb"] = senhaWeb,
            ["CnpjCliente"] = itemSelecionado.CnpjCliente,
            ["DescricaoServico"] = itemSelecionado.DescricaoPersonalizada,
            ["ValorNota"] = itemSelecionado.ValorNota.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["RazaoSocialCliente"] = itemSelecionado.RazaoSocialPlaceholder,
            ["EmailCliente"] = itemSelecionado.EmailCliente,
            ["AnoEmissao"] = hoje.Year.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["MesEmissao"] = hoje.Month.ToString(System.Globalization.CultureInfo.InvariantCulture)
        };

        string numeroNota = ExtrairNumeroNotaConhecido(itemSelecionado.UltimaEmissao);
        if (!string.IsNullOrWhiteSpace(numeroNota))
        {
            dados["NumeroNota"] = numeroNota;
        }

        return dados;
    }

    private static void ValidarContratoCarregado(
        FluxoAutomacaoContrato contrato,
        string receitaPath)
    {
        bool possuiEtapaExecutavel = contrato.Etapas.Any(etapa => etapa.Acoes.Count > 0);

        if (!possuiEtapaExecutavel)
        {
            throw new InvalidOperationException(
                $"O contrato carregado de '{receitaPath}' nao contem etapas executaveis.");
        }
    }

    private static string ExtrairNumeroNotaConhecido(string? valorOriginal)
    {
        if (string.IsNullOrWhiteSpace(valorOriginal))
        {
            return string.Empty;
        }

        return new string(valorOriginal.Where(char.IsDigit).ToArray());
    }
}
