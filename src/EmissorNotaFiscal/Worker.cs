using System.Diagnostics;
using System.Diagnostics.Metrics;
using EmissorNotaFiscal.Application;
using EmissorNotaFiscal.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmissorNotaFiscal;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class Worker : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new("EmissorNotaFiscal.Worker");
    private static readonly Meter Meter = new("EmissorNotaFiscal.Worker");
    private static readonly Counter<long> ExecutionCounter = Meter.CreateCounter<long>(
        "worker.execution.cycles");
    private static readonly Counter<long> ConfigurationFailureCounter = Meter.CreateCounter<long>(
        "worker.configuration.failures");

    private readonly ILogger<Worker> _logger;
    private readonly FaturamentoOrchestrator _orchestrator;
    private readonly WorkerScheduleOptions _scheduleOptions;

    public Worker(
        ILogger<Worker> logger,
        FaturamentoOrchestrator orchestrator,
        IOptions<WorkerScheduleOptions> scheduleOptions)
    {
        _logger = logger;
        _orchestrator = orchestrator;
        _scheduleOptions = scheduleOptions.Value;

        if (_scheduleOptions.IntervalHours <= 0)
        {
            ConfigurationFailureCounter.Add(1);
            throw new OptionsValidationException(
                nameof(WorkerScheduleOptions),
                typeof(WorkerScheduleOptions),
                new[] { "WorkerSchedule:IntervalHours must be greater than zero." });
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TimeSpan executionInterval = TimeSpan.FromHours(_scheduleOptions.IntervalHours);

        _logger.LogInformation(
            "Worker starting with an execution interval of {IntervalHours} hour(s).",
            _scheduleOptions.IntervalHours);

        while (!stoppingToken.IsCancellationRequested)
        {
            using Activity? activity = ActivitySource.StartActivity("worker.execution.cycle");
            DateTimeOffset cycleStartedAt = DateTimeOffset.UtcNow;

            _logger.LogInformation("Worker execution cycle started at {CycleStartedAt}.", cycleStartedAt);

            await _orchestrator.RunAsync(stoppingToken);
            ExecutionCounter.Add(1);

            _logger.LogInformation(
                "Worker execution cycle completed. Waiting {IntervalHours} hour(s) before the next cycle.",
                _scheduleOptions.IntervalHours);

            await Task.Delay(executionInterval, stoppingToken);
        }
    }
}
