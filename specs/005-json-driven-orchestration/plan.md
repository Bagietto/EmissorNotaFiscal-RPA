# Implementation Plan: JSON-Driven Orchestration

**Branch**: `005-json-driven-orchestration` | **Date**: 2026-07-03 | **Spec**: [spec.md](./spec.md)  
**Input**: Feature specification from `/specs/005-json-driven-orchestration/spec.md`

## Summary

Replace the current placeholder automation handoff in `FaturamentoOrchestrator` with a real orchestration path that loads `receita_paulistana.json` and `config_notas_v2.json`, selects one configured billing item, builds the runtime dictionary required by the Playwright engine, and triggers one true test execution.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: existing `Microsoft.Extensions.Logging`, existing `INfeAutomationService`, existing `IConfigRepository`, existing JSON domain models, existing Playwright automation engine  
**Storage**: Local file system JSON files at project root plus existing download output handling in the automation engine  
**Testing**: `dotnet test` with xUnit in follow-up validation work  
**Target Platform**: Windows-hosted .NET 8 Worker runtime with Playwright-capable Chromium automation support  
**Project Type**: Single-project Worker Service with layered folders  
**Performance Goals**: One orchestration cycle should progress from JSON loading to automation invocation without placeholder short-circuiting and without adding unnecessary blocking work outside the existing automation duration  
**Constraints**: Preserve asynchronous flow; reuse existing JSON repository abstractions; select one deterministic billing item only; keep orchestration logic free of Playwright implementation details; do not implement full batch processing in this increment  
**Scale/Scope**: One orchestrator update, one host registration adjustment if needed, one deterministic test-selection rule, and one real JSON-backed automation handoff

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

`.specify/memory/constitution.md` remains a placeholder template with no active enforceable principles. Current gate result: PASS with no constitutional restrictions beyond staying within the approved feature scope.

Post-design re-check: PASS. The design remains limited to orchestration, JSON-backed runtime preparation, and automation handoff.

## Project Structure

### Documentation (this feature)

```text
specs/005-json-driven-orchestration/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── orchestration-runtime-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
EmissorNotaFiscal/
├── Program.cs
├── Application/
│   └── FaturamentoOrchestrator.cs
├── Domain/
│   ├── Interfaces/
│   │   ├── IConfigRepository.cs
│   │   └── INfeAutomationService.cs
│   └── Models/
│       ├── Automation/
│       │   └── FluxoAutomacaoContrato.cs
│       └── Faturamento/
│           └── ConfigFaturamento.cs
├── Infrastructure/
│   ├── Automation/
│   │   └── ContractBasedAutomationEngine.cs
│   └── Storage/
│       └── JsonConfigRepository.cs
├── receita_paulistana.json
└── config_notas_v2.json
```

**Structure Decision**: Keep the current single Worker project and limit code changes primarily to `FaturamentoOrchestrator`, while reusing the existing repository and automation abstractions. The JSON files remain at the project root and are treated as real orchestration inputs for this increment.

## Phase 0: Research Summary

- Confirm the orchestrator should consume the existing `IConfigRepository` abstraction rather than read JSON files directly.
- Confirm the host must register `IConfigRepository` so the orchestrator can load both the billing configuration and automation recipe through the existing storage boundary.
- Confirm one deterministic test-selection rule should be used for billing items in this increment, such as the first configured scheduled item.
- Confirm the runtime dictionary should include the keys already expected by the current automation recipe: `CnpjPrestador`, `SenhaWeb`, `CnpjCliente`, `DescricaoServico`, and `ValorNota`.
- Confirm missing credentials such as `SenhaWeb` may need to come from environment or host configuration because the billing JSON intentionally does not store the web password.

## Phase 1: Design Summary

- Inject `IConfigRepository`, `IConfiguration`, and `INfeAutomationService` into `FaturamentoOrchestrator`.
- Resolve root file paths for `receita_paulistana.json` and `config_notas_v2.json` from the current execution context in a predictable way.
- Load the real automation contract and billing configuration through `IConfigRepository`.
- Select one billing item deterministically for the test path and build the runtime dictionary by combining issuer data, selected billing-item data, and credential/configuration values needed by the current recipe.
- Invoke the automation service with the loaded contract and constructed dictionary, preserving logging and failure propagation.

## Complexity Tracking

No constitutional violations or exceptional complexity require justification at this planning stage.
