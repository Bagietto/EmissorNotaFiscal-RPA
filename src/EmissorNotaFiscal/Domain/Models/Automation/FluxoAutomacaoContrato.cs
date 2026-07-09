using System.Text.Json.Serialization;

namespace EmissorNotaFiscal.Domain.Models.Automation;

public sealed class FluxoAutomacaoContrato
{
    [JsonPropertyName("NomeAutomacao")]
    public string NomeAutomacao { get; init; } = string.Empty;

    [JsonPropertyName("UrlInicial")]
    public string UrlInicial { get; init; } = string.Empty;

    [JsonPropertyName("Etapas")]
    public List<EtapaExecucao> Etapas { get; init; } = [];
}
