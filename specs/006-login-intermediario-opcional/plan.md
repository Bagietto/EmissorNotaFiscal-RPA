# Implementation Plan: Suporte a Login Intermediario Opcional

**Branch**: `[006-login-intermediario-opcional]` | **Date**: 2026-07-03 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/006-login-intermediario-opcional/spec.md`

**Note**: This plan is derived from the existing worker/orchestrator architecture and the current Playwright automation contract.

## Summary

Add optional-step handling to the contract-driven login flow so the automation can proceed whether the intermediate `Login unico` screen appears or not. The work stays concentrated in the Playwright engine and the JSON recipe, preserving the current orchestration and billing pipeline.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: Microsoft.Playwright, Microsoft.Extensions.Hosting, Microsoft.Extensions.Logging, System.Text.Json  
**Storage**: Local JSON files (`receita_paulistana.json`, `config_notas_v2.json`)  
**Testing**: No dedicated test project is present; validation will rely on `dotnet build` plus targeted runtime execution and log inspection  
**Target Platform**: Windows worker process running the existing host  
**Project Type**: Worker/service application with internal automation engine  
**Performance Goals**: Preserve the current execution flow timing and avoid blocking the login path longer than the portal already requires  
**Constraints**: Maintain async-first execution, keep the JSON recipe readable, and avoid hardcoded portal logic outside the automation engine  
**Scale/Scope**: One login branch variation, one JSON recipe update, and one Playwright engine enhancement

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Check | Status | Notes |
|---|---|---|
| Scope is narrow and feature-specific | PASS | The change is limited to optional login handling. |
| Existing architecture is reused | PASS | The worker, orchestrator, repository, and engine remain in place. |
| Async flow is preserved | PASS | The current contract and engine are already async-based. |
| Data flow stays file-backed | PASS | Inputs remain JSON files at the project root. |
| No new external surface is required | PASS | The feature is internal to the worker automation path. |

No constitution violations require justification.

## Project Structure

### Documentation (this feature)

```text
specs/006-login-intermediario-opcional/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── tasks.md
```

### Source Code (repository root)

```text
Application/
├── FaturamentoOrchestrator.cs

Domain/
├── Interfaces/
│   └── INfeAutomationService.cs
└── Models/
    └── Automation/
        ├── AcaoPasso.cs
        ├── EtapaExecucao.cs
        ├── FluxoAutomacaoContrato.cs
        └── TipoAcao.cs

Infrastructure/
└── Automation/
    └── ContractBasedAutomationEngine.cs

receita_paulistana.json
```

**Structure Decision**: Keep the implementation inside the existing worker/application layering. The optional-login behavior belongs in the automation engine and recipe contract, not in the billing orchestrator or storage layer.

## Complexity Tracking

No constitution exceptions are expected for this feature.

