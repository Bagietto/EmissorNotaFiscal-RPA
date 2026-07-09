using System.Text.Json;
using System.Text.Json.Serialization;
using EmissorNotaFiscal.Domain.Interfaces;
using EmissorNotaFiscal.Domain.Models.Automation;
using EmissorNotaFiscal.Domain.Models.Faturamento;

namespace EmissorNotaFiscal.Infrastructure.Storage;

public sealed class JsonConfigRepository : IConfigRepository
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public Task<ConfigFaturamento> ObterConfiguracaoFaturamentoAsync(string caminhoArquivo)
    {
        return LerJsonAsync(
            caminhoArquivo,
            "configuracao de faturamento",
            static () => new ConfigFaturamento());
    }

    public async Task SalvarConfiguracaoFaturamentoAsync(string caminhoArquivo, ConfigFaturamento dados)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(caminhoArquivo);
        ArgumentNullException.ThrowIfNull(dados);

        try
        {
            string? diretorio = Path.GetDirectoryName(caminhoArquivo);

            if (!string.IsNullOrWhiteSpace(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            await using FileStream stream = new(
                caminhoArquivo,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true);

            await JsonSerializer.SerializeAsync(stream, dados, SerializerOptions);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            throw new InvalidOperationException(
                $"Nao foi possivel salvar a configuracao de faturamento no caminho '{caminhoArquivo}'.",
                ex);
        }
    }

    public Task<FluxoAutomacaoContrato> ObterReceitaAutomacaoAsync(string caminhoArquivo)
    {
        return LerJsonAsync<FluxoAutomacaoContrato>(
            caminhoArquivo,
            "receita de automacao",
            onMissingFile: null);
    }

    private static async Task<T> LerJsonAsync<T>(
        string caminhoArquivo,
        string nomeContrato,
        Func<T>? onMissingFile)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(caminhoArquivo);

        if (!File.Exists(caminhoArquivo))
        {
            if (onMissingFile is not null)
            {
                return onMissingFile();
            }

            throw new FileNotFoundException(
                $"O arquivo de {nomeContrato} obrigatorio nao foi encontrado em '{caminhoArquivo}'.",
                caminhoArquivo);
        }

        try
        {
            await using FileStream stream = new(
                caminhoArquivo,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                useAsync: true);

            T? resultado = await JsonSerializer.DeserializeAsync<T>(stream, SerializerOptions);

            return resultado ?? throw new InvalidOperationException(
                $"O arquivo de {nomeContrato} em '{caminhoArquivo}' nao contem um documento JSON valido.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException(
                $"O arquivo de {nomeContrato} em '{caminhoArquivo}' possui JSON invalido ou valores nao suportados.",
                ex);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            throw new InvalidOperationException(
                $"Nao foi possivel ler o arquivo de {nomeContrato} em '{caminhoArquivo}'.",
                ex);
        }
    }
}
