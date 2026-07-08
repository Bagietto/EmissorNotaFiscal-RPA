# Research: Wire Automation Engine

## Decision 1: Register the automation engine through the standard host service collection

- **Decision**: Register `INfeAutomationService` to `ContractBasedAutomationEngine` directly in `Program.cs` using the existing DI bootstrap path.
- **Rationale**: The feature is about runtime wiring, and the current engine already accepts host-friendly dependencies (`ILogger` and optional `IConfiguration`). No extra factory layer is needed yet.
- **Alternatives considered**:
  - Manual construction inside the orchestrator: rejected because it breaks dependency injection and boundary ownership.
  - Dedicated registration extension class: rejected for now because the current scope is too small to justify extra indirection.

## Decision 2: Surface automation settings in `appsettings.json`

- **Decision**: Add an `Automation` section containing explicit `Headless` and `DownloadsDirectory` settings.
- **Rationale**: The engine already expects those configuration keys. Exposing them through the normal application configuration path makes the behavior operable without code changes.
- **Alternatives considered**:
  - Keep implicit engine defaults only: rejected because operators would have no visible configuration seam.
  - Environment-variable-only configuration: rejected because the baseline already uses `appsettings.json` as a first-class config source.

## Decision 3: Update the orchestrator to depend on the abstraction, not the implementation

- **Decision**: Inject `INfeAutomationService` into `FaturamentoOrchestrator` and keep all future automation calls behind that boundary.
- **Rationale**: This preserves the clean architecture intent established in the previous features and keeps Playwright-specific code out of the application layer.
- **Alternatives considered**:
  - Reference `ContractBasedAutomationEngine` directly from the orchestrator: rejected because it couples the application layer to infrastructure.
  - Leave the orchestrator unchanged and register the engine only: rejected because the abstraction would still not participate in the actual runtime path.

## Decision 4: Keep orchestration behavior minimal in this increment

- **Decision**: Prepare the orchestrator to delegate through the automation service but keep the actual invoice workflow handoff minimal until contract-source and runtime-data sourcing are specified in later features.
- **Rationale**: The current feature is about wiring and boundary connection, not inventing end-to-end invoice automation behavior from missing upstream data sources.
- **Alternatives considered**:
  - Fabricate temporary contract and runtime-data inputs: rejected because it introduces speculative workflow behavior.
  - Delay orchestration integration entirely: rejected because the feature explicitly requires connecting the engine to the orchestrator.
