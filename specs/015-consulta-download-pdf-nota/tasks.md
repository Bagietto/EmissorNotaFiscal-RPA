# Tasks: Consulta de Confirmacao e Download do PDF da NFS-e

**Input**: Design documents from `/specs/015-consulta-download-pdf-nota/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: A spec nao exige TDD formal. Validacoes ficam concentradas em `dotnet build` e no roteiro manual de `quickstart.md`.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Phase 1: Setup

**Purpose**: Preparar os contratos minimos e o contexto de runtime que a feature vai consumir

- [x] T001 Mapear os novos dados dinamicos da consulta pos-emissao em `Application/FaturamentoOrchestrator.cs`
- [x] T002 [P] Adicionar os novos tipos de acao do contrato JSON em `Domain/Models/Automation/TipoAcao.cs`

---

## Phase 2: Foundational

**Purpose**: Entregar a infraestrutura compartilhada que bloqueia todas as historias do fluxo pos-emissao

**⚠️ CRITICAL**: Nenhuma historia deve comecar antes desta fase estar pronta

- [x] T003 Implementar contexto de pagina ativa para suportar troca de popup/janela em `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T004 [P] Implementar validacao fail-fast do diretorio de downloads em `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T005 [P] Implementar manipuladores compartilhados de `SelecionarDropdown` e `DispararDownload` em `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T006 Implementar manipuladores compartilhados de `ClicarBotaoEAguardarPopup` e `ClicarLinkContendoDinamico` em `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

**Checkpoint**: A engine ja entende o vocabulário minimo necessario para executar o fluxo de consulta pos-emissao

---

## Phase 3: User Story 1 - Confirmar nota emitida (Priority: P1) 🎯 MVP

**Goal**: Confirmar que a nota emitida aparece na consulta filtrada pelo tomador e periodo corrente

**Independent Test**: Emitir uma nota valida e verificar que a automacao navega para a consulta, filtra por CPF/CNPJ do tomador e por mes/ano correntes, e alcanca o resultado correspondente

- [x] T007 [US1] Adicionar as etapas de navegacao para consulta e filtro por tomador/periodo em `receita_paulistana.json`
- [x] T008 [US1] Popular `AnoEmissao` e `MesEmissao` no dicionario de runtime em `Application/FaturamentoOrchestrator.cs`
- [x] T009 [US1] Garantir que a execucao continua no popup de resultados apos a consulta em `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T010 [US1] Implementar selecao da nota correspondente a execucao atual em `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

**Checkpoint**: A historia 1 fica completa quando a nota recem-emitida pode ser confirmada na tela de consulta sem navegacao manual

---

## Phase 4: User Story 2 - Obter o PDF oficial da nota (Priority: P2)

**Goal**: Abrir a nota confirmada e encerrar a execucao com um PDF oficial fisicamente baixado

**Independent Test**: Com a nota localizada na consulta, a automacao deve abrir a visualizacao da NFS-e e terminar retornando um caminho valido de PDF existente

- [x] T011 [US2] Adicionar as etapas de abertura da nota e disparo do download oficial em `receita_paulistana.json`
- [x] T012 [US2] Estender a captura de downloads para cobrir o contexto da nota aberta via consulta em `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T013 [US2] Preservar o contrato de sucesso baseado em um caminho fisico de PDF em `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

**Checkpoint**: A historia 2 fica completa quando a execucao retorna um PDF real da mesma nota confirmada na consulta

---

## Phase 5: User Story 3 - Falhar com contexto acionavel (Priority: P3)

**Goal**: Expor falhas claras para popup ausente, nota nao localizada, destino invalido ou PDF nao capturado

**Independent Test**: Forcar cenarios de falha na consulta e no download e verificar que a execucao encerra com erro explicito em vez de falso sucesso

- [x] T014 [US3] Falhar antes do login quando o diretorio de downloads nao puder ser preparado em `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T015 [US3] Emitir erro explicito quando o popup de consulta nao abrir ou a nota nao puder ser localizada em `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T016 [US3] Emitir erro explicito quando a nota abrir mas nenhum PDF valido for capturado em `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T017 [US3] Atualizar as orientacoes operacionais de falha e validacao em `specs/015-consulta-download-pdf-nota/quickstart.md`

**Checkpoint**: A historia 3 fica completa quando os principais modos de falha do trecho pos-emissao sao diagnosticaveis pelo operador

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Consolidar documentacao, receita final e validacao local

- [x] T018 [P] Atualizar a documentacao do fluxo orientado a contrato em `DOCUMENTACAO_PROJETO.md`
- [ ] T019 Validar a receita final e os caminhos de sucesso/falha usando `specs/015-consulta-download-pdf-nota/quickstart.md`
- [x] T020 Executar uma validacao de compilacao apos a implementacao a partir de `EmissorNotaFiscal.sln`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1: Setup**: pode iniciar imediatamente
- **Phase 2: Foundational**: depende da conclusao da Phase 1 e bloqueia todas as historias
- **Phase 3: US1**: depende da conclusao da Phase 2
- **Phase 4: US2**: depende da conclusao da Phase 3
- **Phase 5: US3**: depende das bases da Phase 2 e deve ser finalizada antes do fechamento da feature
- **Phase 6: Polish**: depende das historias desejadas concluidas

### User Story Dependencies

- **US1 (P1)**: primeira entrega de valor; define a confirmacao da nota na consulta
- **US2 (P2)**: depende da nota ja estar localizavel pela US1
- **US3 (P3)**: complementa os fluxos anteriores com falhas acionaveis e protecoes operacionais

### Within Each User Story

- Runtime/contexto antes da receita final
- Receita antes da validacao ponta a ponta
- Tratamento de erro antes da validacao final de quickstart

### Parallel Opportunities

- `T001` e `T002` podem ser executadas em paralelo
- `T004` e `T005` podem ser executadas em paralelo apos `T003`
- `T018` pode ocorrer em paralelo com a validacao final quando a implementacao principal estiver estabilizada

---

## Parallel Example: Foundational

```bash
Task: "Implementar validacao fail-fast do diretorio de downloads em Infrastructure/Automation/ContractBasedAutomationEngine.cs"
Task: "Implementar manipuladores compartilhados de SelecionarDropdown e DispararDownload em Infrastructure/Automation/ContractBasedAutomationEngine.cs"
```

---

## Parallel Example: User Story 1

```bash
Task: "Adicionar as etapas de navegacao para consulta e filtro por tomador/periodo em receita_paulistana.json"
Task: "Popular AnoEmissao e MesEmissao no dicionario de runtime em Application/FaturamentoOrchestrator.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Completar Phase 1: Setup
2. Completar Phase 2: Foundational
3. Completar Phase 3: User Story 1
4. Validar que a consulta localiza a nota emitida antes de prosseguir para download

### Incremental Delivery

1. Entregar a base do engine para popup/download
2. Entregar confirmacao da nota na consulta (US1)
3. Entregar download oficial do PDF (US2)
4. Endurecer os cenarios de falha e operacao (US3)

### Suggested MVP Scope

- **MVP recomendado**: concluir ate a Phase 3 (US1) para provar a confirmacao pos-emissao da nota

---

## Notes

- Todas as tasks seguem o formato `- [ ] Txxx ...` com IDs unicos e caminhos explicitos
- Tasks marcadas com `[P]` indicam oportunidade real de paralelismo
- Tasks com `[USx]` estao rastreadas diretamente as historias da spec
- A validacao manual deve seguir `specs/015-consulta-download-pdf-nota/quickstart.md`
