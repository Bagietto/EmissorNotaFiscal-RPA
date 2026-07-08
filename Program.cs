using EmissorNotaFiscal;
using EmissorNotaFiscal.Application;
using EmissorNotaFiscal.Configuration;
using EmissorNotaFiscal.Domain.Interfaces;
using EmissorNotaFiscal.Infrastructure.Automation;
using EmissorNotaFiscal.Infrastructure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(context.HostingEnvironment.ContentRootPath);
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        config.AddEnvironmentVariables();

        if (args is { Length: > 0 })
        {
            config.AddCommandLine(args);
        }
    })
    .ConfigureServices((context, services) =>
    {
        services
            .AddOptions<WorkerScheduleOptions>()
            .Bind(context.Configuration.GetSection(WorkerScheduleOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(
                options => options.IntervalHours > 0,
                "WorkerSchedule:IntervalHours must be greater than zero.")
            .ValidateOnStart();

        services
            .AddOptions<AssistedAutomationOptions>()
            .Bind(context.Configuration.GetSection(AssistedAutomationOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(
                options => options.HumanInterventionTimeoutMinutes > 0,
                "Automation:AssistedMode:HumanInterventionTimeoutMinutes must be greater than zero.")
            .ValidateOnStart();

        services
            .AddOptions<CaptchaSolverOptions>()
            .Bind(context.Configuration.GetSection(CaptchaSolverOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(
                options => !options.Enabled || (!string.IsNullOrWhiteSpace(options.BaseUrl) && !string.IsNullOrWhiteSpace(options.Model)),
                "Automation:CaptchaSolver: BaseUrl and Model are required when Enabled is true.")
            .ValidateOnStart();

        services.AddHttpClient();

        services.AddSingleton<FaturamentoOrchestrator>();
        services.AddSingleton<IConfigRepository, JsonConfigRepository>();
        services.AddSingleton<INfeAutomationService, ContractBasedAutomationEngine>();
        services.AddSingleton<ICaptchaSolverService, OpenAiCompatibleCaptchaSolver>();
        services.AddHostedService<Worker>();

        // Future infrastructure registrations belong here:
        // Storage registration is now active through IConfigRepository -> JsonConfigRepository.
        // Automation registration is now active through INfeAutomationService -> ContractBasedAutomationEngine.
        // services.AddSingleton<IEmailService, MailKitEmailService>();
    })
    .Build();

await host.RunAsync();
