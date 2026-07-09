using EmissorNotaFiscal.Application;
using EmissorNotaFiscal.Domain.Interfaces;
using EmissorNotaFiscal.Domain.Models.Automation;
using EmissorNotaFiscal.Domain.Models.Faturamento;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace EmissorNotaFiscal.Tests;

public sealed class FaturamentoOrchestratorTests : IDisposable
{
    private readonly IConfigRepository _configRepository;
    private readonly INfeAutomationService _automationService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<FaturamentoOrchestrator> _logger;
    private readonly FaturamentoOrchestrator _orchestrator;

    private readonly string _receitaPath;
    private readonly string _configNotasPath;

    public FaturamentoOrchestratorTests()
    {
        _configRepository = Substitute.For<IConfigRepository>();
        _automationService = Substitute.For<INfeAutomationService>();
        _configuration = Substitute.For<IConfiguration>();
        _logger = Substitute.For<ILogger<FaturamentoOrchestrator>>();

        _orchestrator = new FaturamentoOrchestrator(
            _configRepository,
            _automationService,
            _configuration,
            _logger
        );

        // Ensure dummy files exist in BaseDirectory so that ResolveRootFilePath doesn't fail immediately
        _receitaPath = Path.Combine(AppContext.BaseDirectory, "receita_paulistana.json");
        _configNotasPath = Path.Combine(AppContext.BaseDirectory, "config_notas_v2.json");

        File.WriteAllText(_receitaPath, "{}");
        File.WriteAllText(_configNotasPath, "{}");
    }

    public void Dispose()
    {
        if (File.Exists(_receitaPath))
        {
            File.Delete(_receitaPath);
        }
        if (File.Exists(_configNotasPath))
        {
            File.Delete(_configNotasPath);
        }
    }

    [Fact]
    public async Task RunAsync_ShouldSucceed_WhenConfigurationAndServicesAreValid()
    {
        // Arrange
        _configuration["Automation:SenhaWeb"].Returns("minhasenha");

        var contrato = new FluxoAutomacaoContrato
        {
            Etapas = new List<EtapaExecucao>
            {
                new()
                {
                    Acoes = new List<AcaoPasso> { new() }
                }
            }
        };

        var configFaturamento = new ConfigFaturamento
        {
            ConfiguracoesEmissor = new ConfigEmissor
            {
                CnpjPrestador = "11222333000100"
            },
            AgendamentoNotas = new List<ItemNota>
            {
                new()
                {
                    CnpjCliente = "44555666000199",
                    RazaoSocialPlaceholder = "Cliente",
                    EmailCliente = "cliente@example.com",
                    ValorNota = 1500.50m,
                    UltimaEmissao = "NFe 12345"
                }
            }
        };

        _configRepository.ObterReceitaAutomacaoAsync(_receitaPath).Returns(contrato);
        _configRepository.ObterConfiguracaoFaturamentoAsync(_configNotasPath).Returns(configFaturamento);
        _automationService.ExecutarFluxoContratoAsync(contrato, Arg.Any<Dictionary<string, string>>()).Returns("C:\\pdf.pdf");

        // Act
        await _orchestrator.RunAsync(CancellationToken.None);

        // Assert
        await _automationService.Received(1).ExecutarFluxoContratoAsync(
            contrato,
            Arg.Is<Dictionary<string, string>>(d =>
                d["CnpjPrestador"] == "11222333000100" &&
                d["SenhaWeb"] == "minhasenha" &&
                d["CnpjCliente"] == "44555666000199" &&
                d["NumeroNota"] == "12345" &&
                d["ValorNota"] == "1500.50"
            )
        );
    }

    [Fact]
    public async Task RunAsync_ShouldThrowFileNotFoundException_WhenFileIsMissing()
    {
        // Arrange - delete config file to force error in ResolveRootFilePath
        if (File.Exists(_configNotasPath))
        {
            File.Delete(_configNotasPath);
        }

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => _orchestrator.RunAsync(CancellationToken.None));
    }

    [Fact]
    public async Task RunAsync_ShouldThrowInvalidOperationException_WhenContratoHasNoSteps()
    {
        // Arrange
        _configuration["Automation:SenhaWeb"].Returns("senha");

        var contrato = new FluxoAutomacaoContrato
        {
            Etapas = new List<EtapaExecucao>() // Empty steps
        };

        var configFaturamento = new ConfigFaturamento
        {
            ConfiguracoesEmissor = new ConfigEmissor(),
            AgendamentoNotas = new List<ItemNota> { new() }
        };

        _configRepository.ObterReceitaAutomacaoAsync(_receitaPath).Returns(contrato);
        _configRepository.ObterConfiguracaoFaturamentoAsync(_configNotasPath).Returns(configFaturamento);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _orchestrator.RunAsync(CancellationToken.None));
    }

    [Fact]
    public async Task RunAsync_ShouldThrowInvalidOperationException_WhenSenhaWebIsMissing()
    {
        // Arrange
        _configuration["Automation:SenhaWeb"].Returns(string.Empty);

        var contrato = new FluxoAutomacaoContrato
        {
            Etapas = new List<EtapaExecucao>
            {
                new()
                {
                    Acoes = new List<AcaoPasso> { new() }
                }
            }
        };

        var configFaturamento = new ConfigFaturamento
        {
            ConfiguracoesEmissor = new ConfigEmissor(),
            AgendamentoNotas = new List<ItemNota> { new() }
        };

        _configRepository.ObterReceitaAutomacaoAsync(_receitaPath).Returns(contrato);
        _configRepository.ObterConfiguracaoFaturamentoAsync(_configNotasPath).Returns(configFaturamento);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _orchestrator.RunAsync(CancellationToken.None));
    }

    [Fact]
    public async Task RunAsync_ShouldThrowInvalidOperationException_WhenAgendamentoNotasIsEmpty()
    {
        // Arrange
        _configuration["Automation:SenhaWeb"].Returns("senha");

        var contrato = new FluxoAutomacaoContrato
        {
            Etapas = new List<EtapaExecucao>
            {
                new()
                {
                    Acoes = new List<AcaoPasso> { new() }
                }
            }
        };

        var configFaturamento = new ConfigFaturamento
        {
            ConfiguracoesEmissor = new ConfigEmissor(),
            AgendamentoNotas = new List<ItemNota>() // Empty list
        };

        _configRepository.ObterReceitaAutomacaoAsync(_receitaPath).Returns(contrato);
        _configRepository.ObterConfiguracaoFaturamentoAsync(_configNotasPath).Returns(configFaturamento);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _orchestrator.RunAsync(CancellationToken.None));
    }

    [Fact]
    public async Task RunAsync_ShouldPropagateException_WhenRepositoryFails()
    {
        // Arrange
        _configRepository.ObterReceitaAutomacaoAsync(Arg.Any<string>())
            .Returns(Task.FromException<FluxoAutomacaoContrato>(new Exception("Load failed")));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _orchestrator.RunAsync(CancellationToken.None));
        Assert.Equal("Load failed", ex.Message);
    }
}
