using System.ComponentModel.DataAnnotations;

namespace EmissorNotaFiscal.Configuration;

public sealed class WorkerScheduleOptions
{
    public const string SectionName = "WorkerSchedule";

    [Range(1, int.MaxValue, ErrorMessage = "WorkerSchedule:IntervalHours must be greater than zero.")]
    public int IntervalHours { get; init; }
}
