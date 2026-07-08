# Implementation Plan: Modo Assistido para Caption no PMSP Auth

**Branch**: `[012-modo-assistido-caption-pmspauth]` | **Date**: 2026-07-04 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/012-modo-assistido-caption-pmspauth/spec.md`

## Summary

Adicionar um modo assistido configuravel para o fluxo de autenticacao no `pmspauth`, mantendo o navegador visivel, detectando sinais estruturais de `mcaptcha`, pausando o worker para intervencao humana e retomando apenas quando houver continuidade funcional valida.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: Microsoft.Playwright, Microsoft.Extensions.Logging, Microsoft.Extensions.Configuration, Microsoft.Extensions.Options  
**Storage**: `appsettings.json`, `Configuration/`, `Infrastructure/Automation/ContractBasedAutomationEngine.cs`, plus spec artifacts in `specs/012-modo-assistido-caption-pmspauth/`  
**Testing**: Build validation plus runtime execution/log inspection in assisted mode  
**Target Platform**: Windows worker process  
**Project Type**: Worker/service application  
**Performance Goals**: Poll the assisted state cheaply, keep the browser open only while needed, and avoid resuming before the portal is actually usable again  
**Constraints**: No OCR, no audio solving, no third-party captcha bypass, and no hardcoded authentication shortcut  
**Scale/Scope**: One engine enhancement plus configuration surface for assisted execution

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Check | Status | Notes |
|---|---|---|
| Scope is narrow and bounded | PASS | Focado em espera assistida no `pmspauth`. |
| Existing automation contract remains intact | PASS | A receita continua dirigindo o fluxo principal. |
| Async browser flow is preserved | PASS | A pausa assistida usa polling assíncrono. |
| No hardcoded provider URL is introduced | PASS | O fluxo segue observando a navegacao real. |
| Logging improves diagnostics | PASS | Entrada, permanencia e timeout do modo assistido serao registrados. |

## Project Structure

### Documentation (this feature)

```text
specs/012-modo-assistido-caption-pmspauth/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── tasks.md
└── checklists/
    └── requirements.md
```

### Source Code (repository root)

```text
Configuration/
└── AssistedAutomationOptions.cs

Infrastructure/
└── Automation/
    └── ContractBasedAutomationEngine.cs

appsettings.json
```

**Structure Decision**: Implementar o comportamento assistido no engine, porque o problema envolve estado do navegador, tempo de espera e retomada condicional. Usar `appsettings` e linha de comando via configuração existente para controlar a feature.
