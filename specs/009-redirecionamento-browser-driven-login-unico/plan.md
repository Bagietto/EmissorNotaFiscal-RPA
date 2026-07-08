# Implementation Plan: Redirecionamento Browser-Driven do Login Unico

**Branch**: `[009-redirecionamento-browser-driven-login-unico]` | **Date**: 2026-07-03 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/009-redirecionamento-browser-driven-login-unico/spec.md`

## Summary

Teach the automation engine to treat the Login Unico click as a browser-driven redirect, waiting for the real navigation and logging the final URL before continuing to credential steps.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: Microsoft.Playwright, Microsoft.Extensions.Logging, Microsoft.Extensions.Configuration  
**Storage**: Existing JSON recipe files  
**Testing**: Build validation plus runtime execution/log inspection  
**Target Platform**: Windows worker process  
**Project Type**: Worker/service application  
**Performance Goals**: Wait only as long as the browser needs for the redirect and avoid extra polling beyond the initial navigation window  
**Constraints**: Preserve async flow, avoid hardcoded auth URLs, and keep the current optional-login recipe intact  
**Scale/Scope**: One engine behavior enhancement and optional recipe selector refinement

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Check | Status | Notes |
|---|---|---|
| Scope is narrow and bounded | PASS | Focused on redirect handling after one click. |
| Existing automation contract remains intact | PASS | The recipe still drives the flow. |
| Async browser flow is preserved | PASS | Playwright awaits are used for redirect handling. |
| No hardcoded provider URL is introduced | PASS | The redirect is browser-driven. |
| Logging improves diagnostics | PASS | Final URL is emitted after navigation. |

## Project Structure

### Documentation (this feature)

```text
specs/009-redirecionamento-browser-driven-login-unico/
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

receita_paulistana.json
```

**Structure Decision**: Implement the redirect wait in the engine, since the browser needs to observe the actual navigation after the click. Keep recipe updates minimal and explicit.

