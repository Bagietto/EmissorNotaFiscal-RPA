# Implementation Plan: Correcao do Seletor da Tela de Login Unico

**Branch**: `[008-correcao-seletor-login-unico]` | **Date**: 2026-07-03 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/008-correcao-seletor-login-unico/spec.md`

## Summary

Update the optional login step in `receita_paulistana.json` to use a structural selector that matches the real Login Unico button, preserving the current optional-step behavior and downstream login flow.

## Technical Context

**Language/Version**: JSON recipe update for the existing .NET 8 worker  
**Primary Dependencies**: Existing Playwright engine and current JSON contract models  
**Storage**: Local recipe file `receita_paulistana.json`  
**Testing**: Build validation plus runtime execution/log inspection  
**Target Platform**: Windows worker process  
**Project Type**: Worker/service application  
**Performance Goals**: No measurable impact beyond improving the optional step match rate  
**Constraints**: No engine rewrite, no orchestrator change, no recipe contract reshaping  
**Scale/Scope**: One JSON selector correction

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Check | Status | Notes |
|---|---|---|
| Scope is narrowly bounded | PASS | Only the Login Unico selector changes. |
| Existing optional-path behavior is preserved | PASS | The step remains optional. |
| Existing engine behavior remains untouched | PASS | No engine code change is required. |
| Contract structure remains stable | PASS | Only the selector value changes. |

## Project Structure

### Documentation (this feature)

```text
specs/008-correcao-seletor-login-unico/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── tasks.md
```

### Source Code (repository root)

```text
receita_paulistana.json
Infrastructure/Automation/ContractBasedAutomationEngine.cs
```

**Structure Decision**: Make the correction in the recipe file only. The engine already supports optional clicks and the problem is the recipe selector mismatch.

