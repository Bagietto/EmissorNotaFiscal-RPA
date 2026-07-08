using System.ComponentModel.DataAnnotations;

namespace EmissorNotaFiscal.Configuration;

public sealed class CaptchaSolverOptions
{
    public const string SectionName = "Automation:CaptchaSolver";

    public bool Enabled { get; init; }

    [Required(ErrorMessage = "Automation:CaptchaSolver:BaseUrl is required when CaptchaSolver is enabled.")]
    public string BaseUrl { get; init; } = string.Empty;

    public string ApiKey { get; init; } = string.Empty;

    [Required(ErrorMessage = "Automation:CaptchaSolver:Model is required when CaptchaSolver is enabled.")]
    public string Model { get; init; } = string.Empty;

    [Range(1, 10, ErrorMessage = "Automation:CaptchaSolver:MaxRetries must be between 1 and 10.")]
    public int MaxRetries { get; init; } = 3;

    [Range(5, 120, ErrorMessage = "Automation:CaptchaSolver:TimeoutSeconds must be between 5 and 120.")]
    public int TimeoutSeconds { get; init; } = 15;

    [Required(ErrorMessage = "Automation:CaptchaSolver:Selectors is required.")]
    public CaptchaSelectors Selectors { get; init; } = new();
}

public sealed class CaptchaSelectors
{
    [Required(ErrorMessage = "Automation:CaptchaSolver:Selectors:CaptchaImage is required.")]
    public string CaptchaImage { get; init; } = string.Empty;

    [Required(ErrorMessage = "Automation:CaptchaSolver:Selectors:InputResponse is required.")]
    public string InputResponse { get; init; } = string.Empty;

    [Required(ErrorMessage = "Automation:CaptchaSolver:Selectors:SubmitButton is required.")]
    public string SubmitButton { get; init; } = string.Empty;

    public string ReloadButton { get; init; } = string.Empty;
}
