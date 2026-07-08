# Implementation Plan: Ajuste do Seletor do Botao Entrar e Consolidacao das Investigacoes de Login

**Branch**: `[010-ajuste-botao-entrar-login]` | **Date**: 2026-07-04 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/010-ajuste-botao-entrar-login/spec.md`

## Summary

Refinar o seletor do passo final de login na `receita_paulistana.json` para que a automacao clique apenas no botao tradicional de submit, preservando o fluxo ja estabilizado ate a tela real de autenticacao e consolidando o historico das investigacoes de login.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: Microsoft.Playwright, Microsoft.Extensions.Logging, Microsoft.Extensions.Configuration  
**Storage**: Local JSON recipe file `receita_paulistana.json` plus spec artifacts in `specs/010-ajuste-botao-entrar-login/`  
**Testing**: Build validation plus targeted runtime execution/log inspection  
**Target Platform**: Windows worker process  
**Project Type**: Worker/service application  
**Performance Goals**: Eliminar falha imediata por seletor ambiguo sem introduzir espera extra no clique final  
**Constraints**: Preservar o fluxo browser-driven do `Login unico`, nao alterar os campos de credenciais e evitar seletor textual fragil  
**Scale/Scope**: Um refinamento na receita de login e documentacao consolidada do estado atual da investigacao

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Check | Status | Notes |
|---|---|---|
| Scope is narrow and bounded | PASS | O ajuste principal esta no seletor final de login. |
| Existing automation contract remains intact | PASS | A receita continua dirigindo o fluxo. |
| Async browser flow is preserved | PASS | Nenhuma mudanca no comportamento async atual e necessaria. |
| No hardcoded provider URL is introduced | PASS | O redirect browser-driven existente permanece inalterado. |
| Logging improves diagnostics | PASS | A documentacao consolidada melhora o contexto operacional. |

## Project Structure

### Documentation (this feature)

```text
specs/010-ajuste-botao-entrar-login/
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
receita_paulistana.json
Infrastructure/
└── Automation/
    └── ContractBasedAutomationEngine.cs
```

**Structure Decision**: Aplicar a correcao diretamente na receita enquanto o motor permanece como fallback caso a tela real exija no futuro um criterio adicional de clique.
