using System.Text.Json.Serialization;

namespace EmissorNotaFiscal.Domain.Models.Faturamento;

public sealed class ConfigEmissor
{
    [JsonPropertyName("cnpj_prestador")]
    public string CnpjPrestador { get; init; } = string.Empty;

    [JsonPropertyName("codigo_servico_paulistana")]
    public string CodigoServicoPaulistana { get; init; } = string.Empty;
}
