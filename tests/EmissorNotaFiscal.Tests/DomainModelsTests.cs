using EmissorNotaFiscal.Domain.Models.Automation;
using EmissorNotaFiscal.Domain.Models.Faturamento;

namespace EmissorNotaFiscal.Tests;

public sealed class DomainModelsTests
{
    [Fact]
    public void AcaoPasso_ShouldInitializePropertiesCorrectly()
    {
        // Arrange & Act
        var acao = new AcaoPasso
        {
            Descricao = "Clique no login",
            PlaywrightAcao = TipoAcao.ClicarBotao,
            SeletorHtml = "#btn-login",
            ValorDinamicoChave = "Senha",
            Opcional = true
        };

        // Assert
        Assert.Equal("Clique no login", acao.Descricao);
        Assert.Equal(TipoAcao.ClicarBotao, acao.PlaywrightAcao);
        Assert.Equal("#btn-login", acao.SeletorHtml);
        Assert.Equal("Senha", acao.ValorDinamicoChave);
        Assert.True(acao.Opcional);
    }

    [Fact]
    public void EtapaExecucao_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var acoes = new List<AcaoPasso>
        {
            new() { Descricao = "Acao 1" }
        };

        // Act
        var etapa = new EtapaExecucao
        {
            Ordem = 1,
            NomeEtapa = "Login",
            Opcional = false,
            Acoes = acoes
        };

        // Assert
        Assert.Equal(1, etapa.Ordem);
        Assert.Equal("Login", etapa.NomeEtapa);
        Assert.False(etapa.Opcional);
        Assert.Same(acoes, etapa.Acoes);
    }

    [Fact]
    public void FluxoAutomacaoContrato_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var etapas = new List<EtapaExecucao>
        {
            new() { NomeEtapa = "Etapa 1" }
        };

        // Act
        var contrato = new FluxoAutomacaoContrato
        {
            NomeAutomacao = "Receita Paulistana",
            UrlInicial = "https://example.com",
            Etapas = etapas
        };

        // Assert
        Assert.Equal("Receita Paulistana", contrato.NomeAutomacao);
        Assert.Equal("https://example.com", contrato.UrlInicial);
        Assert.Same(etapas, contrato.Etapas);
    }

    [Fact]
    public void ConfigEmissor_ShouldInitializePropertiesCorrectly()
    {
        // Arrange & Act
        var config = new ConfigEmissor
        {
            CnpjPrestador = "12345678901234",
            CodigoServicoPaulistana = "03020"
        };

        // Assert
        Assert.Equal("12345678901234", config.CnpjPrestador);
        Assert.Equal("03020", config.CodigoServicoPaulistana);
    }

    [Fact]
    public void ConfigFaturamento_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var configEmissor = new ConfigEmissor { CnpjPrestador = "123" };
        var agendamentos = new List<ItemNota>
        {
            new() { CnpjCliente = "456" }
        };

        // Act
        var config = new ConfigFaturamento
        {
            ConfiguracoesEmissor = configEmissor,
            AgendamentoNotas = agendamentos
        };

        // Assert
        Assert.Same(configEmissor, config.ConfiguracoesEmissor);
        Assert.Same(agendamentos, config.AgendamentoNotas);
    }

    [Fact]
    public void ItemNota_ShouldInitializePropertiesCorrectly()
    {
        // Arrange & Act
        var item = new ItemNota
        {
            CnpjCliente = "98765432109876",
            RazaoSocialPlaceholder = "Cliente Teste",
            EmailCliente = "cliente@example.com",
            ValorNota = 1500.50m,
            DiaEmissao = 10,
            DescricaoPersonalizada = "Servicos Prestados",
            UltimaEmissao = "Nota 123"
        };

        // Assert
        Assert.Equal("98765432109876", item.CnpjCliente);
        Assert.Equal("Cliente Teste", item.RazaoSocialPlaceholder);
        Assert.Equal("cliente@example.com", item.EmailCliente);
        Assert.Equal(1500.50m, item.ValorNota);
        Assert.Equal(10, item.DiaEmissao);
        Assert.Equal("Servicos Prestados", item.DescricaoPersonalizada);
        Assert.Equal("Nota 123", item.UltimaEmissao);
    }
}
