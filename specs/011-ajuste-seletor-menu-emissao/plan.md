# Implementation Plan: Ajuste de Encoding e Seletor do Menu de Emissao

**Branch**: `[011-ajuste-seletor-menu-emissao]` | **Date**: 2026-07-04 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/011-ajuste-seletor-menu-emissao/spec.md`

## Summary

Substituir o seletor textual do passo `Acessar Menu Lateral Emissao` por um seletor estrutural baseado no destino `nota.aspx` e no identificador de menu observado, preservando o fluxo autenticado e reduzindo a fragilidade a encoding `iso-8859-1`.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: Microsoft.Playwright, Microsoft.Extensions.Logging, Microsoft.Extensions.Configuration  
**Storage**: Local JSON recipe file `receita_paulistana.json` plus spec artifacts in `specs/011-ajuste-seletor-menu-emissao/`  
**Testing**: Build validation plus targeted runtime execution/log inspection  
**Target Platform**: Windows worker process  
**Project Type**: Worker/service application  
**Performance Goals**: Eliminar timeout do clique no menu lateral sem introduzir navegacao extra ou waits artificiais  
**Constraints**: Preservar o fluxo de login ja estabilizado, evitar dependencia de texto acentuado, e manter o engine inalterado se a receita for suficiente  
**Scale/Scope**: Um refinamento de seletor na receita e a documentacao do ajuste incremental

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Check | Status | Notes |
|---|---|---|
| Scope is narrow and bounded | PASS | O ajuste esta concentrado no clique do menu lateral. |
| Existing automation contract remains intact | PASS | A receita continua sendo a origem do comportamento. |
| Async browser flow is preserved | PASS | Nenhuma alteracao no fluxo async do motor e necessaria. |
| No hardcoded provider URL is introduced | PASS | O login e o redirect anteriores permanecem inalterados. |
| Logging improves diagnostics | PASS | A especificacao registra o motivo do abandono do seletor textual. |

## Project Structure

### Documentation (this feature)

```text
specs/011-ajuste-seletor-menu-emissao/
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

**Structure Decision**: Aplicar o ajuste no contrato JSON com um seletor estrutural composto, mantendo o motor como esta porque a falha atual decorre do locator configurado e nao do mecanismo de clique.
