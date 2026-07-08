using EmissorNotaFiscal.Domain.Models.Automation;

namespace EmissorNotaFiscal.Domain.Interfaces;

public interface INfeAutomationService
{
    /// <summary>
    /// Executa um fluxo de automacao orientado a contrato e retorna o caminho fisico do PDF
    /// baixado durante a execucao.
    /// </summary>
    Task<string> ExecutarFluxoContratoAsync(
        FluxoAutomacaoContrato contrato,
        Dictionary<string, string> dicionarioDadosReais);
}
