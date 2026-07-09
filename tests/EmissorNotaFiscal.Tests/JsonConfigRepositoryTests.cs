using EmissorNotaFiscal.Domain.Models.Automation;
using EmissorNotaFiscal.Domain.Models.Faturamento;
using EmissorNotaFiscal.Infrastructure.Storage;

namespace EmissorNotaFiscal.Tests;

public sealed class JsonConfigRepositoryTests : IDisposable
{
    private readonly JsonConfigRepository _repository;
    private readonly string _tempFilePath;

    public JsonConfigRepositoryTests()
    {
        _repository = new JsonConfigRepository();
        _tempFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".json");
    }

    public void Dispose()
    {
        if (File.Exists(_tempFilePath))
        {
            File.Delete(_tempFilePath);
        }
    }

    [Fact]
    public async Task ObterConfiguracaoFaturamentoAsync_ShouldReturnDefaultConfig_WhenFileDoesNotExist()
    {
        // Act
        ConfigFaturamento result = await _repository.ObterConfiguracaoFaturamentoAsync(_tempFilePath);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.AgendamentoNotas);
        Assert.NotNull(result.ConfiguracoesEmissor);
    }

    [Fact]
    public async Task ObterConfiguracaoFaturamentoAsync_ShouldReturnConfig_WhenFileExistsAndIsValid()
    {
        // Arrange
        string json = """
        {
          "configuracoes_emissor": {
            "cnpj_prestador": "12345",
            "codigo_servico_paulistana": "03020"
          },
          "agendamento_notas": [
            {
              "cnpj_cliente": "67890",
              "valor_nota": 100.50
            }
          ]
        }
        """;
        await File.WriteAllTextAsync(_tempFilePath, json);

        // Act
        ConfigFaturamento result = await _repository.ObterConfiguracaoFaturamentoAsync(_tempFilePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("12345", result.ConfiguracoesEmissor.CnpjPrestador);
        Assert.Equal("03020", result.ConfiguracoesEmissor.CodigoServicoPaulistana);
        Assert.Single(result.AgendamentoNotas);
        Assert.Equal("67890", result.AgendamentoNotas[0].CnpjCliente);
        Assert.Equal(100.50m, result.AgendamentoNotas[0].ValorNota);
    }

    [Fact]
    public async Task ObterConfiguracaoFaturamentoAsync_ShouldThrowInvalidOperationException_WhenFileHasInvalidJson()
    {
        // Arrange
        await File.WriteAllTextAsync(_tempFilePath, "invalid json");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _repository.ObterConfiguracaoFaturamentoAsync(_tempFilePath));
    }

    [Fact]
    public async Task SalvarConfiguracaoFaturamentoAsync_ShouldSaveFileSuccessfully()
    {
        // Arrange
        var config = new ConfigFaturamento
        {
            ConfiguracoesEmissor = new ConfigEmissor { CnpjPrestador = "999" },
            AgendamentoNotas = new List<ItemNota>
            {
                new() { CnpjCliente = "888", ValorNota = 250m }
            }
        };

        // Act
        await _repository.SalvarConfiguracaoFaturamentoAsync(_tempFilePath, config);

        // Assert
        Assert.True(File.Exists(_tempFilePath));
        string savedJson = await File.ReadAllTextAsync(_tempFilePath);
        Assert.Contains("999", savedJson);
        Assert.Contains("888", savedJson);

        // Reload and verify
        ConfigFaturamento reloaded = await _repository.ObterConfiguracaoFaturamentoAsync(_tempFilePath);
        Assert.Equal("999", reloaded.ConfiguracoesEmissor.CnpjPrestador);
        Assert.Equal("888", reloaded.AgendamentoNotas[0].CnpjCliente);
    }

    [Fact]
    public async Task ObterReceitaAutomacaoAsync_ShouldThrowFileNotFoundException_WhenFileDoesNotExist()
    {
        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            _repository.ObterReceitaAutomacaoAsync(_tempFilePath));
    }

    [Fact]
    public async Task ObterReceitaAutomacaoAsync_ShouldReturnContrato_WhenFileExistsAndIsValid()
    {
        // Arrange
        string json = """
        {
          "NomeAutomacao": "Teste",
          "UrlInicial": "http://localhost",
          "Etapas": [
            {
              "Ordem": 1,
              "NomeEtapa": "Login",
              "Acoes": [
                {
                  "Descricao": "Acao 1",
                  "PlaywrightAcao": "ClicarBotao"
                }
              ]
            }
          ]
        }
        """;
        await File.WriteAllTextAsync(_tempFilePath, json);

        // Act
        FluxoAutomacaoContrato result = await _repository.ObterReceitaAutomacaoAsync(_tempFilePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Teste", result.NomeAutomacao);
        Assert.Equal("http://localhost", result.UrlInicial);
        Assert.Single(result.Etapas);
        Assert.Equal("Login", result.Etapas[0].NomeEtapa);
        Assert.Single(result.Etapas[0].Acoes);
        Assert.Equal(TipoAcao.ClicarBotao, result.Etapas[0].Acoes[0].PlaywrightAcao);
    }

    [Fact]
    public async Task SalvarConfiguracaoFaturamentoAsync_ShouldThrowArgumentException_WhenPathIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _repository.SalvarConfiguracaoFaturamentoAsync("", new ConfigFaturamento()));
    }

    [Fact]
    public async Task SalvarConfiguracaoFaturamentoAsync_ShouldThrowArgumentNullException_WhenDataIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _repository.SalvarConfiguracaoFaturamentoAsync(_tempFilePath, null!));
    }
}
