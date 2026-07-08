# Tasks: Modo Assistido para Caption no PMSP Auth

**Input**: Design documents from `/specs/012-modo-assistido-caption-pmspauth/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md

## Phase 1: Setup

- [x] T001 Review the current `pmspauth` login flow and its post-login failure evidence
- [x] T002 Review the current automation configuration surface in `Program.cs` and `appsettings.json`

## Phase 2: Foundational

- [x] T003 Add assisted-mode configuration binding and validation in `Configuration/` and `Program.cs`
- [x] T004 Update active feature pointers in `.specify/feature.json` and `AGENTS.md`

## Phase 3: User Story 1 - Pausar o fluxo para resolucao humana do challenge (Priority: P1)

- [x] T005 [US1] Force visible browser execution when assisted mode is enabled in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T006 [US1] Detect `mcaptcha`-style challenge markers and enter controlled wait mode in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

## Phase 4: User Story 2 - Retomar automaticamente apos a intervencao humana (Priority: P2)

- [x] T007 [US2] Resume only after a valid continuation signal is observed in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T008 [US2] Log assisted wait entry and successful resume in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

## Phase 5: User Story 3 - Encerrar com timeout claro quando nao houver resposta humana (Priority: P3)

- [x] T009 [US3] Apply configurable assisted-wait timeout in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T010 [US3] Capture timeout diagnostics before failing the flow in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

## Phase 6: Polish

- [x] T011 [P] Create the specification quality checklist in `specs/012-modo-assistido-caption-pmspauth/checklists/requirements.md`
- [x] T012 Validate the integrated assisted-mode changes with a local build check
