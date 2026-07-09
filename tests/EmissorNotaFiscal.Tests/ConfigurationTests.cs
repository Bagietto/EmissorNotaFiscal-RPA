using System.ComponentModel.DataAnnotations;
using EmissorNotaFiscal.Configuration;

namespace EmissorNotaFiscal.Tests;

public sealed class ConfigurationTests
{
    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(model, serviceProvider: null, items: null);
        Validator.TryValidateObject(model, context, validationResults, validateAllProperties: true);
        return validationResults;
    }

    [Fact]
    public void WorkerScheduleOptions_ShouldBeInvalid_WhenIntervalHoursIsZeroOrNegative()
    {
        // Arrange
        var optionsZero = new WorkerScheduleOptions { IntervalHours = 0 };
        var optionsNegative = new WorkerScheduleOptions { IntervalHours = -5 };
        var optionsValid = new WorkerScheduleOptions { IntervalHours = 1 };

        // Act
        var resultsZero = ValidateModel(optionsZero);
        var resultsNegative = ValidateModel(optionsNegative);
        var resultsValid = ValidateModel(optionsValid);

        // Assert
        Assert.Single(resultsZero);
        Assert.Equal("WorkerSchedule:IntervalHours must be greater than zero.", resultsZero[0].ErrorMessage);

        Assert.Single(resultsNegative);
        Assert.Equal("WorkerSchedule:IntervalHours must be greater than zero.", resultsNegative[0].ErrorMessage);

        Assert.Empty(resultsValid);
    }

    [Fact]
    public void AssistedAutomationOptions_ShouldBeInvalid_WhenTimeoutIsZeroOrNegative()
    {
        // Arrange
        var optionsZero = new AssistedAutomationOptions { HumanInterventionTimeoutMinutes = 0 };
        var optionsValid = new AssistedAutomationOptions { HumanInterventionTimeoutMinutes = 5 };

        // Act
        var resultsZero = ValidateModel(optionsZero);
        var resultsValid = ValidateModel(optionsValid);

        // Assert
        Assert.Single(resultsZero);
        Assert.Equal("Automation:AssistedMode:HumanInterventionTimeoutMinutes must be greater than zero.", resultsZero[0].ErrorMessage);
        Assert.Empty(resultsValid);
    }

    [Fact]
    public void CaptchaSolverOptions_ShouldValidateSuccessfully_WhenAllRequiredFieldsAreValid()
    {
        // Arrange
        var options = new CaptchaSolverOptions
        {
            Enabled = true,
            BaseUrl = "https://vision-api.example.com",
            ApiKey = "my-secret-key",
            Model = "gpt-4-vision",
            MaxRetries = 3,
            TimeoutSeconds = 15,
            Selectors = new CaptchaSelectors
            {
                CaptchaImage = "#img",
                InputResponse = "#input",
                SubmitButton = "#submit",
                ReloadButton = "#reload"
            }
        };

        // Act
        var results = ValidateModel(options);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void CaptchaSolverOptions_ShouldBeInvalid_WhenRequiredPropertiesAreMissing()
    {
        // Arrange
        var options = new CaptchaSolverOptions
        {
            Enabled = true,
            BaseUrl = "", // Required
            Model = "", // Required
            MaxRetries = 15, // Out of range (1 to 10)
            TimeoutSeconds = 3, // Out of range (5 to 120)
            Selectors = new CaptchaSelectors
            {
                CaptchaImage = "", // Required
                InputResponse = "", // Required
                SubmitButton = "" // Required
            }
        };

        // Act
        var results = ValidateModel(options);

        // Assert
        Assert.Contains(results, r => r.ErrorMessage == "Automation:CaptchaSolver:BaseUrl is required when CaptchaSolver is enabled.");
        Assert.Contains(results, r => r.ErrorMessage == "Automation:CaptchaSolver:Model is required when CaptchaSolver is enabled.");
        Assert.Contains(results, r => r.ErrorMessage == "Automation:CaptchaSolver:MaxRetries must be between 1 and 10.");
        Assert.Contains(results, r => r.ErrorMessage == "Automation:CaptchaSolver:TimeoutSeconds must be between 5 and 120.");
    }
}
