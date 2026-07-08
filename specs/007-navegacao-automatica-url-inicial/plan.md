# Implementation Plan: Navegacao Automatica pela UrlInicial

**Branch**: `[007-navegacao-automatica-url-inicial]` | **Date**: 2026-07-03 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/007-navegacao-automatica-url-inicial/spec.md`

## Summary

Move the contract bootstrap into the automation engine so the first page open happens automatically from `FluxoAutomacaoContrato.UrlInicial`, while keeping later `Navegar` steps intact.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: Microsoft.Playwright, Microsoft.Extensions.Logging, Microsoft.Extensions.Configuration  
**Storage**: Existing JSON recipe files  
**Testing**: Build validation plus runtime execution/log inspection  
**Target Platform**: Windows worker process  
**Project Type**: Worker/service application  
**Performance Goals**: Bootstrap navigation should add no extra blocking work beyond the initial page load  
**Constraints**: Keep async flow, keep `Navegar` available for later steps, fail clearly when `UrlInicial` is missing or invalid  
**Scale/Scope**: One engine behavior change and no contract reshaping

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Check | Status | Notes |
|---|---|---|
| Scope is narrow and focused | PASS | Only initial navigation behavior changes. |
| Existing layering is preserved | PASS | Change stays in the engine. |
| Async behavior is preserved | PASS | Playwright calls remain async. |
| Recipe compatibility is preserved | PASS | Existing `Navegar` steps remain supported. |
| Clear failure behavior exists | PASS | Missing or invalid `UrlInicial` is an explicit error. |

## Project Structure

### Documentation (this feature)

```text
specs/007-navegacao-automatica-url-inicial/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── tasks.md
```

### Source Code (repository root)

```text
Infrastructure/
└── Automation/
    └── ContractBasedAutomationEngine.cs

Domain/
└── Models/
    └── Automation/
        └── FluxoAutomacaoContrato.cs

receita_paulistana.json
```

**Structure Decision**: Keep the fix inside the existing automation engine. The contract already exposes the initial URL, so the engine should own the bootstrap navigation.

