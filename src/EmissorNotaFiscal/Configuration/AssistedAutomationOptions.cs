using System.ComponentModel.DataAnnotations;

namespace EmissorNotaFiscal.Configuration;

public sealed class AssistedAutomationOptions
{
    public const string SectionName = "Automation:AssistedMode";

    public bool Enabled { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Automation:AssistedMode:HumanInterventionTimeoutMinutes must be greater than zero.")]
    public int HumanInterventionTimeoutMinutes { get; init; } = 10;
}
