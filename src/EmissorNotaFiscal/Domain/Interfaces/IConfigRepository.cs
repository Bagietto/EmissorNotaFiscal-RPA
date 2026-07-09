using EmissorNotaFiscal.Domain.Models.Automation;
using EmissorNotaFiscal.Domain.Models.Faturamento;

namespace EmissorNotaFiscal.Domain.Interfaces;

public interface IConfigRepository
{
    /// <summary>
    /// Carrega a configuracao de faturamento do disco ou retorna uma estrutura vazia segura
    /// quando o arquivo ainda nao existe.
    /// </summary>
    Task<ConfigFaturamento> ObterConfiguracaoFaturamentoAsync(string caminhoArquivo);

    /// <summary>
    /// Persiste a configuracao de faturamento em formato JSON identado.
    /// </summary>
    Task SalvarConfiguracaoFaturamentoAsync(string caminhoArquivo, ConfigFaturamento dados);

    /// <summary>
    /// Carrega a receita de automacao obrigatoria do disco.
    /// </summary>
    Task<FluxoAutomacaoContrato> ObterReceitaAutomacaoAsync(string caminhoArquivo);
}
