# Tasks: Correcao do Seletor da Tela de Login Unico

**Input**: Design documents from `/specs/008-correcao-seletor-login-unico/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md

## Phase 1: Setup

- [x] T001 Review the current Login Unico entry in `receita_paulistana.json`
- [x] T002 Confirm the optional login path remains intact in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

## Phase 2: Foundational

- [x] T003 Update the Login Unico selector to the structural button selector in `receita_paulistana.json`
- [x] T004 Preserve the optional flag on the Login Unico step in `receita_paulistana.json`

## Phase 3: User Story 1 - Clicar corretamente no Login Unico (Priority: P1)

- [x] T005 [US1] Replace the text-based selector with `button.oauth-button` in `receita_paulistana.json`
- [x] T006 [US1] Validate that the optional action still targets the intermediate login button in `receita_paulistana.json`

## Phase 4: User Story 2 - Avancar para a tela de credenciais (Priority: P2)

- [x] T007 [US2] Confirm the recipe still allows the flow to progress to `#cpfCnpj` and `#password` after the optional step in `receita_paulistana.json`

## Phase 5: Polish

- [x] T008 [P] Update the usage notes to reflect the selector correction in `specs/008-correcao-seletor-login-unico/quickstart.md`
- [x] T009 Validate the integrated flow against `specs/008-correcao-seletor-login-unico/plan.md`
