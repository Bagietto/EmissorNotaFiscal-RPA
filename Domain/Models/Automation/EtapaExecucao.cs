using System.Text.Json.Serialization;

namespace EmissorNotaFiscal.Domain.Models.Automation;

public sealed class EtapaExecucao
{
    [JsonPropertyName("Ordem")]
    public int Ordem { get; init; }

    [JsonPropertyName("NomeEtapa")]
    public string NomeEtapa { get; init; } = string.Empty;

    [JsonPropertyName("Opcional")]
    public bool Opcional { get; init; }

    [JsonPropertyName("Acoes")]
    public List<AcaoPasso> Acoes { get; init; } = [];
}
