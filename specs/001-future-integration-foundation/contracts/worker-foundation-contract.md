# Contract: Worker Foundation Baseline

## Scope

This contract defines the observable baseline responsibilities for the first implementation increment of the Worker Service foundation.

## Startup Contract

- The application must boot through the standard .NET host builder.
- The application must load `appsettings.json`.
- The application must register the hosted worker service.
- The application must register the orchestration entry point used by the worker.
- The application must contain clearly labeled registration seams for future storage, automation, and email infrastructure services.

## Configuration Contract

- A recurring execution interval setting must be present in configuration.
- Missing or invalid interval configuration must stop normal startup or recurring execution readiness with a clear error.
- Configuration binding must be centralized rather than scattered across runtime code.

## Execution Contract

- The worker must run a safe cancellation-aware loop.
- Each loop iteration must invoke the orchestration seam.
- The worker must wait according to the configured interval between completed iterations.
- The orchestration seam may be structurally empty in this increment.

## Boundary Contract

- `Infrastructure/Storage`, `Infrastructure/Automation`, and `Infrastructure/Email` are reserved for future adapters only.
- This increment must not add placeholder implementations per infrastructure area.
- No Playwright or MailKit business logic is allowed in this increment.

## Observability Contract

- Startup behavior must emit identifiable log events.
- Configuration validation failures must emit identifiable failure signals.
- Execution-cycle start/end behavior must emit identifiable operational signals.
- The codebase must expose reserved seams for metrics and tracing expansion, even if detailed telemetry exporters are deferred.
