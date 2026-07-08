# Implementation Plan: Contract-Driven Playwright Automation

**Branch**: `003-playwright-contract-engine` | **Date**: 2026-07-03 | **Spec**: [spec.md](./spec.md)  
**Input**: Feature specification from `/specs/003-playwright-contract-engine/spec.md`

## Summary

Add a generic asynchronous automation service boundary plus a contract-driven Playwright engine that interprets existing automation recipes, injects runtime values, handles downloads and browser dialogs, and surfaces step-level failures with explicit execution context.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: `Microsoft.Playwright`, existing `Microsoft.Extensions.Logging`, existing domain contract models from the previous feature  
**Storage**: Local file system for Playwright download output only; no new persistence store  
**Testing**: `dotnet test` with xUnit in follow-up validation work  
**Target Platform**: Windows-hosted .NET 8 Worker runtime with Chromium automation support  
**Project Type**: Single-project Worker Service with layered folders  
**Performance Goals**: A single contract run should complete with predictable step sequencing and without blocking non-automation host flow beyond the duration of the browser interactions themselves  
**Constraints**: All automation APIs must be asynchronous; no portal-specific selectors or hardcoded workflow branches; browser must clean up on success and failure; headless mode must remain configurable; failure handling must identify the failed stage and action  
**Scale/Scope**: One automation service abstraction, one Playwright-based engine implementation, reuse of the existing automation contract model, and one focused execution/download handling increment

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

`.specify/memory/constitution.md` remains a placeholder template with no active enforceable principles. Current gate result: PASS with no constitutional restrictions beyond staying within the approved feature scope.

Post-design re-check: PASS. The design remains limited to the automation boundary, Playwright execution engine, and operational error handling required by this feature.

## Project Structure

### Documentation (this feature)

```text
specs/003-playwright-contract-engine/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── automation-service-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
EmissorNotaFiscal/
├── EmissorNotaFiscal.csproj
├── Domain/
│   ├── Interfaces/
│   │   └── INfeAutomationService.cs
│   └── Models/
│       └── Automation/
│           ├── AcaoPasso.cs
│           ├── EtapaExecucao.cs
│           ├── FluxoAutomacaoContrato.cs
│           └── TipoAcao.cs
└── Infrastructure/
    └── Automation/
        └── ContractBasedAutomationEngine.cs
```

**Structure Decision**: Keep the existing single Worker project and extend the reserved automation area with one concrete Playwright engine. The service abstraction remains in `Domain/Interfaces`, while browser-specific execution stays isolated under `Infrastructure/Automation` and consumes the already-created contract models.

## Phase 0: Research Summary

- Confirm Playwright lifecycle management should be encapsulated per contract execution using async disposal for the Playwright instance, browser, context, and page.
- Confirm download handling should rely on Playwright download events and a controlled download directory so the engine can return one physical PDF path without portal-specific logic.
- Confirm runtime data resolution should fail explicitly when a required dynamic key is absent rather than filling with empty values.
- Confirm step-level error wrapping should preserve stage order, stage name, and action description so the orchestrator receives actionable execution context.
- Confirm headless configurability can be delivered without adding full orchestration wiring by keeping the engine configurable through construction-time options or explicit parameters owned by the infrastructure layer.

## Phase 1: Design Summary

- Model one automation run around a contract input plus a runtime data dictionary and one resulting PDF path.
- Define `INfeAutomationService` as the application-facing asynchronous dispatch boundary for contract execution.
- Implement `ContractBasedAutomationEngine` with ordered stage execution, action dispatch through a `switch` on `TipoAcao`, and shared helper methods for repeated Playwright interactions.
- Capture downloads generically through browser-context download handling and validate that a PDF path exists before reporting success.
- Introduce a dedicated automation exception type within the automation implementation scope so step failures are surfaced with execution context instead of raw browser exceptions alone.

## Complexity Tracking

No constitutional violations or exceptional complexity require justification at this planning stage.
