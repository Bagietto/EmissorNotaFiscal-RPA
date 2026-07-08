using System.Text.Json.Serialization;

namespace EmissorNotaFiscal.Domain.Models.Automation;

public sealed class AcaoPasso
{
    [JsonPropertyName("Descricao")]
    public string Descricao { get; init; } = string.Empty;

    [JsonPropertyName("PlaywrightAcao")]
    public TipoAcao PlaywrightAcao { get; init; }

    [JsonPropertyName("SeletorHtml")]
    public string SeletorHtml { get; init; } = string.Empty;

    [JsonPropertyName("ValorDinamicoChave")]
    public string? ValorDinamicoChave { get; init; }

    [JsonPropertyName("Opcional")]
    public bool Opcional { get; init; }
}
