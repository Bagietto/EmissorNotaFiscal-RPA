using System.Text.Json.Serialization;

namespace EmissorNotaFiscal.Domain.Models.Faturamento;

public sealed class ItemNota
{
    [JsonPropertyName("cnpj_cliente")]
    public string CnpjCliente { get; init; } = string.Empty;

    [JsonPropertyName("razao_social_placeholder")]
    public string RazaoSocialPlaceholder { get; init; } = string.Empty;

    [JsonPropertyName("email_cliente")]
    public string EmailCliente { get; init; } = string.Empty;

    [JsonPropertyName("valor_nota")]
    public decimal ValorNota { get; init; }

    [JsonPropertyName("dia_emissao")]
    public int DiaEmissao { get; init; }

    [JsonPropertyName("descricao_personalizada")]
    public string DescricaoPersonalizada { get; init; } = string.Empty;

    [JsonPropertyName("ultima_emissao")]
    public string UltimaEmissao { get; init; } = string.Empty;
}
