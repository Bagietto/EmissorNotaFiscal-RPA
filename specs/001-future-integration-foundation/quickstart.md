# Quickstart: Foundation for Future Integrations

## Prerequisites

- .NET 8 SDK installed
- Windows PowerShell or compatible shell
- NuGet connectivity available for package restore

## Expected Project Outcome

After implementation, the repository should contain a compilable Worker Service baseline with:

- root project file targeting .NET 8
- `Program.cs` using the standard host builder
- `Worker.cs` with validated recurring execution configuration
- `Application/FaturamentoOrchestrator.cs` as the orchestration seam
- reserved `Domain` and `Infrastructure` folders
- explicit configuration file with required interval setting
- comments or seams for future infrastructure DI registrations
- logging, metrics, and tracing extension points visible in startup and worker flow
- `WorkerSchedule:IntervalHours` configured with a positive integer value

## Build

```powershell
dotnet restore
dotnet build
```

## Run

```powershell
dotnet run --project .\EmissorNotaFiscal.csproj
```

## Configuration Notes

- The required schedule section is `WorkerSchedule`.
- The required setting is `IntervalHours`.
- Missing, empty, zero, negative, or non-convertible interval values must fail startup validation.

## Validation Checklist

- Confirm build succeeds without adding Playwright or MailKit business logic.
- Confirm the worker fails fast if the required execution interval is missing or invalid.
- Confirm the startup registers the hosted service and orchestrator seam.
- Confirm the repository tree includes `Application`, `Domain`, `Infrastructure`, and configuration artifacts.
- Confirm logging exists for startup, configuration validation outcomes, and execution-cycle boundaries.
- Confirm named seams or comments exist for future metrics and tracing wiring.
- Confirm no concrete Playwright or MailKit integration logic exists in the baseline.

## Next Step

Run `/speckit-tasks` to decompose this plan into implementation tasks.
