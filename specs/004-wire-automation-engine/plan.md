# Implementation Plan: Wire Automation Engine

**Branch**: `004-wire-automation-engine` | **Date**: 2026-07-03 | **Spec**: [spec.md](./spec.md)  
**Input**: Feature specification from `/specs/004-wire-automation-engine/spec.md`

## Summary

Wire the existing Playwright automation engine into the host dependency graph, expose its runtime configuration through `appsettings.json`, and update the application orchestrator so browser automation is invoked through the `INfeAutomationService` abstraction instead of remaining a placeholder-only boundary.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: existing `Microsoft.Extensions.Hosting`, `Microsoft.Extensions.Configuration`, `Microsoft.Extensions.Logging`, `Microsoft.Playwright`, existing automation abstraction and engine types  
**Storage**: Local file system configuration plus local download directory settings already consumed by the automation engine  
**Testing**: `dotnet test` with xUnit in follow-up validation work  
**Target Platform**: Windows-hosted .NET 8 Worker runtime with Playwright-capable Chromium automation support  
**Project Type**: Single-project Worker Service with layered folders  
**Performance Goals**: Host startup remains predictable while the worker and orchestrator resolve the automation dependency through DI without adding blocking startup work beyond normal dependency construction  
**Constraints**: Preserve asynchronous flow; keep `FaturamentoOrchestrator` dependent only on `INfeAutomationService`; expose headless and download-directory settings through normal host configuration; do not introduce portal-specific automation rules in this increment  
**Scale/Scope**: One host registration path, one orchestrator dependency update, one minimal configuration expansion, and no additional external systems

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

`.specify/memory/constitution.md` remains a placeholder template with no active enforceable principles. Current gate result: PASS with no constitutional restrictions beyond staying within the approved feature scope.

Post-design re-check: PASS. The design remains limited to DI wiring, application-boundary integration, and runtime configuration exposure.

## Project Structure

### Documentation (this feature)

```text
specs/004-wire-automation-engine/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── automation-wiring-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
EmissorNotaFiscal/
├── EmissorNotaFiscal.csproj
├── Program.cs
├── appsettings.json
├── Application/
│   └── FaturamentoOrchestrator.cs
├── Domain/
│   └── Interfaces/
│       └── INfeAutomationService.cs
└── Infrastructure/
    └── Automation/
        └── ContractBasedAutomationEngine.cs
```

**Structure Decision**: Keep the current single Worker project and limit changes to the host bootstrap, application orchestrator, and runtime configuration file. The automation abstraction and engine created in earlier features are reused without moving their ownership boundaries.

## Phase 0: Research Summary

- Confirm the existing automation engine can be registered directly as a singleton or scoped runtime dependency through the host without adding special factory infrastructure.
- Confirm the orchestrator should depend on `INfeAutomationService` only and remain free of direct Playwright implementation references.
- Confirm the minimal runtime configuration needed in `appsettings.json` is the same `Automation:Headless` and `Automation:DownloadsDirectory` contract already consumed by the engine.
- Confirm a minimal orchestration handoff in this feature should remain behavior-light and avoid inventing a full invoice workflow before contract-source and runtime-data source features exist.

## Phase 1: Design Summary

- Register `INfeAutomationService` to `ContractBasedAutomationEngine` in `Program.cs` using the existing host bootstrap path.
- Extend `appsettings.json` with explicit automation settings for browser visibility and download location.
- Update `FaturamentoOrchestrator` to receive `INfeAutomationService` through constructor injection and shift its behavior from a placeholder-only boundary to an automation-aware application boundary.
- Keep the orchestrator decoupled from portal-specific automation details by stopping at abstraction-based delegation and explicit logging/error propagation seams.

## Complexity Tracking

No constitutional violations or exceptional complexity require justification at this planning stage.
