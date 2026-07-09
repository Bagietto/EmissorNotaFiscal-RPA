using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EmissorNotaFiscal.Configuration;
using EmissorNotaFiscal.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmissorNotaFiscal.Infrastructure.Automation;

public sealed class OpenAiCompatibleCaptchaSolver : ICaptchaSolverService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenAiCompatibleCaptchaSolver> _logger;
    private readonly CaptchaSolverOptions _options;

    public OpenAiCompatibleCaptchaSolver(
        HttpClient httpClient,
        ILogger<OpenAiCompatibleCaptchaSolver> logger,
        IOptions<CaptchaSolverOptions> options)
    {
        _httpClient = httpClient;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<string> SolveAsync(string base64Image, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(base64Image))
        {
            throw new ArgumentException("A imagem base64 do captcha não pode ser vazia.", nameof(base64Image));
        }

        if (!_options.Enabled)
        {
            _logger.LogWarning("O resolvedor de captcha está desativado nas configurações.");
            return string.Empty;
        }

        _logger.LogInformation("Iniciando chamada para a API de Vision no endpoint {BaseUrl} usando o modelo {Model}.", _options.BaseUrl, _options.Model);

        // Define a URL base completa do data URL
        string dataUrl = base64Image.StartsWith("data:", StringComparison.OrdinalIgnoreCase) 
            ? base64Image 
            : $"data:image/png;base64,{base64Image}";

        var payload = new
        {
            model = _options.Model,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new
                        {
                            type = "text",
                            text = "Analise a imagem de captcha fornecida. Retorne EXCLUSIVAMENTE os caracteres alfanuméricos contidos na imagem, sem formatação, sem espaços adicionais, sem explicações ou texto complementar. Sua resposta deve ser apenas o código do captcha resolvido."
                        },
                        new
                        {
                            type = "image_url",
                            image_url = new
                            {
                                url = dataUrl
                            }
                        }
                    }
                }
            }
        };

        string jsonPayload = JsonSerializer.Serialize(payload);

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.BaseUrl);
        request.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        if (!string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        }

        request.Headers.Add("HTTP-Referer", "https://github.com/Antigravity/EmissorNotaFiscal");

        // Timeout customizado
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(_options.TimeoutSeconds));

        try
        {
            using HttpResponseMessage response = await _httpClient.SendAsync(request, cts.Token);
            
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync(cts.Token);
                _logger.LogError("A API de Vision respondeu com código de erro {StatusCode}. Detalhes: {Details}", response.StatusCode, errorContent);
                throw new HttpRequestException($"API de Vision retornou status {response.StatusCode}: {errorContent}");
            }

            string responseBody = await response.Content.ReadAsStringAsync(cts.Token);
            _logger.LogDebug("Resposta recebida da API de Vision: {ResponseBody}", responseBody);

            using JsonDocument doc = JsonDocument.Parse(responseBody);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("choices", out JsonElement choices) && 
                choices.ValueKind == JsonValueKind.Array && 
                choices.GetArrayLength() > 0)
            {
                JsonElement firstChoice = choices[0];
                if (firstChoice.TryGetProperty("message", out JsonElement message) && 
                    message.TryGetProperty("content", out JsonElement contentProperty))
                {
                    string? rawContent = contentProperty.GetString();
                    if (!string.IsNullOrWhiteSpace(rawContent))
                    {
                        // Limpa a resposta obtendo apenas caracteres alfanuméricos (letras e números)
                        string cleaned = new string(rawContent.Where(char.IsLetterOrDigit).ToArray());
                        _logger.LogInformation("Captcha resolvido com sucesso pela IA. Resposta crua: '{Raw}', Resposta limpa: '{Cleaned}'", rawContent, cleaned);
                        return cleaned;
                    }
                }
            }

            _logger.LogWarning("O JSON retornado pela API não possui a estrutura esperada de escolhas.");
            return string.Empty;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError("A requisição para a API de Vision estourou o limite de tempo configurado de {TimeoutSeconds} segundos.", _options.TimeoutSeconds);
            throw new TimeoutException($"A requisição para o resolvedor de captcha excedeu o timeout de {_options.TimeoutSeconds}s.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao realizar chamada para a API de resolvedor de captcha.");
            throw;
        }
    }
}
