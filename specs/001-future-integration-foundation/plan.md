# Implementation Plan: Foundation for Future Integrations

**Branch**: `001-future-integration-foundation` | **Date**: 2026-07-03 | **Spec**: [spec.md](./spec.md)  
**Input**: Feature specification from `/specs/001-future-integration-foundation/spec.md`

## Summary

Create a .NET 8 Worker Service baseline that reserves the application, domain, and infrastructure boundaries for future invoice-processing integrations, while intentionally limiting the first delivery to startup wiring, periodic execution scaffolding, explicit configuration validation, and observability-ready extension points.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: `Microsoft.Extensions.Hosting`, `Microsoft.Playwright`, `MailKit`, `Microsoft.Extensions.Options.ConfigurationExtensions`, `Microsoft.Extensions.Options.DataAnnotations`, `Microsoft.Extensions.Logging` as compile-time/runtime host dependencies only; `Microsoft.Playwright` and `MailKit` are referenced for future readiness, not active integration behavior in this increment  
**Storage**: Local file system only for future structural reservation; no active persistence in this increment  
**Testing**: `dotnet test` with xUnit for unit coverage of startup/configuration behavior in follow-up tasks  
**Target Platform**: Windows-hosted .NET Worker runtime, portable to standard .NET 8 supported environments  
**Project Type**: Worker Service  
**Performance Goals**: Service startup completes with validated configuration and enters a predictable wait-execute cycle without blocking host initialization beyond normal dependency loading  
**Constraints**: No business logic for Playwright or MailKit; no per-integration placeholder adapters; recurring interval must be explicitly configured and validated before loop start; observability expectations must be visible from the baseline; no authentication/authorization behavior added in this increment; accepted structural placeholders are directories, `.gitkeep` files, and descriptive `README.md` files only  
**Scale/Scope**: Single worker process, one orchestrator entry point, three reserved infrastructure areas (`Storage`, `Automation`, `Email`), intentionally minimal `Domain/Models` and `Domain/Interfaces` reservation areas, and configuration/startup foundation only

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

`.specify/memory/constitution.md` is still a placeholder template and defines no enforceable project principles yet. Current gate result: PASS with no active constitutional restrictions to validate beyond staying aligned with the approved feature scope.

Post-design re-check: PASS. The design remains within the clarified scope by limiting delivery to structure, startup, configuration validation, and observability-ready seams.

## Project Structure

### Documentation (this feature)

```text
specs/001-future-integration-foundation/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── worker-foundation-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
EmissorNotaFiscal/
├── EmissorNotaFiscal.csproj
├── Program.cs
├── Worker.cs
├── appsettings.json
├── Application/
│   └── FaturamentoOrchestrator.cs
├── Domain/
│   ├── Interfaces/
│   └── Models/
├── Infrastructure/
│   ├── Automation/
│   ├── Email/
│   └── Storage/
└── Configuration/
    └── WorkerScheduleOptions.cs

tests/
└── EmissorNotaFiscal.Tests/
    ├── Unit/
    └── Integration/
```

**Structure Decision**: Use a single Worker Service project at the repository root, with architectural folders inside the project for `Application`, `Domain`, `Infrastructure`, and `Configuration`. Keep `Infrastructure` directories free of concrete adapters in this increment, allowing only non-behavioral markers such as `.gitkeep` and `README.md`, and reserve a separate `tests/` area for follow-up validation work.

## Phase 0: Research Summary

- Confirm Worker Service startup pattern for .NET 8 using `Host.CreateDefaultBuilder(args)` plus explicit `appsettings.json` loading.
- Confirm strongly typed configuration with startup validation is the best fit for an explicitly required execution interval.
- Confirm observability baseline should expose seams for logs, metrics, and tracing without implementing domain telemetry pipelines yet.
- Confirm `Playwright` and `MailKit` package references may be included now without introducing functional integration code.

## Phase 1: Design Summary

- Model the baseline around a single orchestration entry point (`FaturamentoOrchestrator`) and a single hosted background service (`Worker`).
- Treat the required recurring interval as configuration data that must be present and validated before normal execution, with missing, empty, zero, negative, or non-convertible values rejected before the loop starts.
- Preserve structural boundaries for `Infrastructure/Storage`, `Infrastructure/Automation`, and `Infrastructure/Email` with registration comments in `Program.cs` instead of concrete adapters; directory-only structure or non-behavioral marker files are valid.
- Define the startup path as host creation, `appsettings.json` loading, required options binding/validation, core service registration, and transition into worker-managed recurring execution.
- Surface observability through a dedicated design requirement: identifiable startup logs, configuration-failure logs, execution-cycle boundary logs, reserved metric names, and reserved trace activity names, while detailed telemetry exporters remain deferred.

## Complexity Tracking

No constitutional violations or exceptional complexity require justification at this planning stage.
