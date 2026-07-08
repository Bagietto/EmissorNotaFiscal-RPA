# Tasks: Redirecionamento Browser-Driven do Login Unico

**Input**: Design documents from `/specs/009-redirecionamento-browser-driven-login-unico/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md

## Phase 1: Setup

- [x] T001 Review the current Login Unico click handling in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T002 Review the current recipe selector for Login Unico in `receita_paulistana.json`

## Phase 2: Foundational

- [x] T003 Add redirect-wait support after Login Unico clicks in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T004 Add final-URL logging after Login Unico redirects in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

## Phase 3: User Story 1 - Seguir o redirect real do navegador (Priority: P1)

- [x] T005 [US1] Wait for browser navigation after the Login Unico click in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T006 [US1] Ensure the flow resumes only after the redirect completes in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

## Phase 4: User Story 2 - Registrar a URL final apos o redirecionamento (Priority: P2)

- [x] T007 [US2] Log the final redirect URL in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T008 [US2] Include diagnostic context when the redirect does not complete in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

## Phase 5: User Story 3 - Preservar os seletores da tela final (Priority: P3)

- [x] T009 [US3] Confirm the credential steps run after the redirect in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T010 [US3] Keep the optional Login Unico selector compatible in `receita_paulistana.json`

## Phase 6: Polish

- [x] T011 [P] Update the quickstart guidance in `specs/009-redirecionamento-browser-driven-login-unico/quickstart.md`
- [x] T012 Validate the integrated flow against `specs/009-redirecionamento-browser-driven-login-unico/plan.md`
