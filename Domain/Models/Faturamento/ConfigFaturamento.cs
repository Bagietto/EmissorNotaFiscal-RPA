using System.Text.Json.Serialization;

namespace EmissorNotaFiscal.Domain.Models.Faturamento;

public sealed class ConfigFaturamento
{
    [JsonPropertyName("configuracoes_emissor")]
    public ConfigEmissor ConfiguracoesEmissor { get; init; } = new();

    [JsonPropertyName("agendamento_notas")]
    public List<ItemNota> AgendamentoNotas { get; init; } = [];
}
