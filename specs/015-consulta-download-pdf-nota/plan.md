# Implementation Plan: Consulta de Confirmacao e Download do PDF da NFS-e

**Branch**: `[015-consulta-download-pdf-nota]` | **Date**: 2026-07-09 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/015-consulta-download-pdf-nota/spec.md`

## Summary

Estender o fluxo atual de emissao para confirmar a nota recem-emitida pela tela de consultas do portal, localizar a nota correspondente ao tomador e periodo corrente, abrir sua visualizacao e baixar o PDF oficial como criterio final de sucesso da automacao. A implementacao exigira evolucao coordenada da receita JSON e do motor Playwright para suportar filtro por periodo, popup de resultados, selecao da nota certa e validacao antecipada do diretorio de download.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: Microsoft.Playwright, Microsoft.Extensions.Configuration, Microsoft.Extensions.Logging, Microsoft.Extensions.Options  
**Storage**: Arquivos JSON locais para receita/configuracao e arquivos PDF no sistema de arquivos  
**Testing**: `dotnet build`, validacao manual assistida do fluxo de emissao e consulta, e testes de unidade para novos comportamentos isolaveis do engine quando aplicavel  
**Target Platform**: Windows worker process  
**Project Type**: Background Worker Service orientado a contrato JSON  
**Performance Goals**: Confirmar a nota e obter o PDF dentro da mesma execucao operacional, sem introduzir esperas indefinidas apos a emissao  
**Constraints**:
- Preservar a arquitetura atual orientada a contrato, mantendo detalhes de DOM e navegador restritos a `Infrastructure/Automation`.
- Nao transformar a feature em consulta historica ampla; o alvo continua sendo a nota emitida na execucao corrente.
- Falhar antes do login quando o diretorio de downloads nao puder ser utilizado.
- Continuar compatível com o fluxo de Login Unico e com o mecanismo atual de captura de download PDF.
**Scale/Scope**: Uma unica automacao de emissao por execucao, com confirmacao pos-emissao de um documento e um PDF oficial por rodada

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Check | Status | Notes |
|---|---|---|
| Existe constituicao efetiva no repo para aplicar gates reais? | PASS | `.specify/memory/constitution.md` ainda esta no template padrao, sem principios normativos preenchidos. |
| A feature respeita o isolamento entre contrato/domain e engine/infrastructure? | PASS | O plano mantem os contratos no modelo JSON/domain e a navegacao Playwright no engine. |
| O retorno continua baseado em PDF fisico baixado? | PASS | O sucesso continua condicionado ao mesmo contrato operacional ja existente no servico. |
| O escopo permanece restrito a emissao corrente? | PASS | O plano exclui explicitamente consultas historicas em lote. |

## Project Structure

### Documentation (this feature)

```text
specs/015-consulta-download-pdf-nota/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── post-emission-confirmation-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
appsettings.json
Application/
└── FaturamentoOrchestrator.cs
Configuration/
Domain/
├── Interfaces/
│   └── INfeAutomationService.cs
└── Models/
    ├── Automation/
    │   ├── AcaoPasso.cs
    │   ├── EtapaExecucao.cs
    │   ├── FluxoAutomacaoContrato.cs
    │   └── TipoAcao.cs
    └── Faturamento/
        └── ItemNota.cs
Infrastructure/
├── Automation/
│   └── ContractBasedAutomationEngine.cs
└── Storage/
    └── JsonConfigRepository.cs
receita_paulistana.json
config_notas_v2.json
```

**Structure Decision**: Permanecer no projeto unico atual, com mudancas concentradas em `receita_paulistana.json`, no orquestrador para fornecer dados de contexto da emissao e no `ContractBasedAutomationEngine` para suportar o trecho pos-emissao sem violar a separacao entre contrato e execucao do navegador.
