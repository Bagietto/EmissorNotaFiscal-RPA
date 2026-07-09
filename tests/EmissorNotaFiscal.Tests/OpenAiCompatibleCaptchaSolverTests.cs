using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using EmissorNotaFiscal.Configuration;
using EmissorNotaFiscal.Infrastructure.Automation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace EmissorNotaFiscal.Tests;

public sealed class OpenAiCompatibleCaptchaSolverTests
{
    private readonly ILogger<OpenAiCompatibleCaptchaSolver> _logger;
    private readonly MockHttpMessageHandler _httpHandler;
    private readonly HttpClient _httpClient;

    public OpenAiCompatibleCaptchaSolverTests()
    {
        _logger = Substitute.For<ILogger<OpenAiCompatibleCaptchaSolver>>();
        _httpHandler = new MockHttpMessageHandler();
        _httpClient = new HttpClient(_httpHandler);
    }

    private IOptions<CaptchaSolverOptions> CreateOptions(bool enabled, int timeoutSeconds = 15)
    {
        var options = new CaptchaSolverOptions
        {
            Enabled = enabled,
            BaseUrl = "https://api.openai.com/v1/chat/completions",
            ApiKey = "test-api-key",
            Model = "gpt-4-vision-preview",
            TimeoutSeconds = timeoutSeconds,
            Selectors = new CaptchaSelectors
            {
                CaptchaImage = "#img",
                InputResponse = "#input",
                SubmitButton = "#submit"
            }
        };

        var mock = Substitute.For<IOptions<CaptchaSolverOptions>>();
        mock.Value.Returns(options);
        return mock;
    }

    [Fact]
    public async Task SolveAsync_ShouldThrowArgumentException_WhenBase64ImageIsEmpty()
    {
        // Arrange
        var options = CreateOptions(enabled: true);
        var solver = new OpenAiCompatibleCaptchaSolver(_httpClient, _logger, options);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => solver.SolveAsync("", CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(() => solver.SolveAsync("   ", CancellationToken.None));
    }

    [Fact]
    public async Task SolveAsync_ShouldReturnEmptyString_WhenSolverIsDisabled()
    {
        // Arrange
        var options = CreateOptions(enabled: false);
        var solver = new OpenAiCompatibleCaptchaSolver(_httpClient, _logger, options);

        // Act
        string result = await solver.SolveAsync("dGVzdA==", CancellationToken.None);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public async Task SolveAsync_ShouldReturnCleanedCaptchaText_WhenApiCallIsSuccessful()
    {
        // Arrange
        var options = CreateOptions(enabled: true);
        var responsePayload = new
        {
            choices = new[]
            {
                new
                {
                    message = new
                    {
                        content = " XyZ 1 2 3 - \n"
                    }
                }
            }
        };

        _httpHandler.SendAsyncHandler = (request, token) =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal(options.Value.BaseUrl, request.RequestUri!.ToString());
            Assert.Equal("Bearer", request.Headers.Authorization!.Scheme);
            Assert.Equal("test-api-key", request.Headers.Authorization.Parameter);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(responsePayload))
            };
            return Task.FromResult(response);
        };

        var solver = new OpenAiCompatibleCaptchaSolver(_httpClient, _logger, options);

        // Act
        string result = await solver.SolveAsync("dGVzdA==", CancellationToken.None);

        // Assert
        Assert.Equal("XyZ123", result); // Only letters and digits
    }

    [Fact]
    public async Task SolveAsync_ShouldThrowHttpRequestException_WhenApiReturnsErrorStatusCode()
    {
        // Arrange
        var options = CreateOptions(enabled: true);
        _httpHandler.SendAsyncHandler = (request, token) =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Invalid Vision Input API Error")
            };
            return Task.FromResult(response);
        };

        var solver = new OpenAiCompatibleCaptchaSolver(_httpClient, _logger, options);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => solver.SolveAsync("dGVzdA==", CancellationToken.None));
        Assert.Contains("API de Vision retornou status BadRequest", ex.Message);
    }

    [Fact]
    public async Task SolveAsync_ShouldThrowTimeoutException_WhenApiRequestTimesOut()
    {
        // Arrange
        var options = CreateOptions(enabled: true, timeoutSeconds: 5);
        _httpHandler.SendAsyncHandler = (request, token) =>
        {
            // Simulate task cancellation from timeout
            throw new OperationCanceledException(token);
        };

        var solver = new OpenAiCompatibleCaptchaSolver(_httpClient, _logger, options);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<TimeoutException>(() => solver.SolveAsync("dGVzdA==", CancellationToken.None));
        Assert.Contains("A requisição para o resolvedor de captcha excedeu o timeout", ex.Message);
    }

    [Fact]
    public async Task SolveAsync_ShouldReturnEmptyString_WhenResponseJsonIsInvalidStructure()
    {
        // Arrange
        var options = CreateOptions(enabled: true);
        _httpHandler.SendAsyncHandler = (request, token) =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}") // Missing choice property
            };
            return Task.FromResult(response);
        };

        var solver = new OpenAiCompatibleCaptchaSolver(_httpClient, _logger, options);

        // Act
        string result = await solver.SolveAsync("dGVzdA==", CancellationToken.None);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    private sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        public Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>? SendAsyncHandler { get; set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (SendAsyncHandler is null)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            }
            return SendAsyncHandler(request, cancellationToken);
        }
    }
}
