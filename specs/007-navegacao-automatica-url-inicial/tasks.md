# Tasks: Navegacao Automatica pela UrlInicial

**Input**: Design documents from `/specs/007-navegacao-automatica-url-inicial/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md

**Tests**: No dedicated test tasks were requested for this feature increment.

## Phase 1: Setup

- [x] T001 Review the current bootstrap flow in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T002 Review the contract root URL usage in `Domain/Models/Automation/FluxoAutomacaoContrato.cs`
- [x] T003 Review the current recipe navigation actions in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

## Phase 2: Foundational

- [x] T004 Add automatic bootstrap navigation before recipe stages in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T005 Add validation and clear failure handling for missing or invalid `UrlInicial` in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T006 Add log output for the initial navigation bootstrap in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

## Phase 3: User Story 1 - Abrir a pagina inicial automaticamente (Priority: P1)

- [x] T007 [US1] Execute `GotoAsync` for `contrato.UrlInicial` before iterating recipe stages in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T008 [US1] Ensure the automatic initial navigation happens only once per execution in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

## Phase 4: User Story 2 - Manter logs e falhas coerentes com o bootstrap (Priority: P2)

- [x] T009 [US2] Log the bootstrap URL used by the engine in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T010 [US2] Surface a clear configuration error when `UrlInicial` is absent or invalid in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

## Phase 5: User Story 3 - Preservar navegacoes adicionais na receita (Priority: P3)

- [x] T011 [US3] Preserve existing `Navegar` action handling for later stage transitions in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T012 [US3] Validate that automatic bootstrap navigation does not suppress recipe-defined navigation in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

## Phase 6: Polish

- [x] T013 [P] Update the usage notes to reflect the automatic bootstrap behavior in `specs/007-navegacao-automatica-url-inicial/quickstart.md`
- [x] T014 Validate the integrated flow against `specs/007-navegacao-automatica-url-inicial/plan.md`
