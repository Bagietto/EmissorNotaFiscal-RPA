# Tasks: Finalizacao da Emissao com Confirmacao e Download do PDF

**Input**: Design documents from `/specs/013-finalizacao-emissao-download-pdf/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md

## Phase 1: Setup

- [x] T001 Review the current end-of-flow recipe gap after the financial fields
- [x] T002 Review the engine expectations for dialogs and PDF download completion

## Phase 2: Foundational

- [x] T003 Update active feature pointers in `.specify/feature.json` and `AGENTS.md`
- [x] T004 Capture the end-of-flow download failure context in the new spec artifacts

## Phase 3: User Story 1 - Disparar a emissao final da nota (Priority: P1)

- [x] T005 [US1] Add a final emission step to `receita_paulistana.json`
- [x] T006 [US1] Use a structural selector strategy for the final emission control in `receita_paulistana.json`

## Phase 4: User Story 2 - Compatibilizar a confirmacao exigida pelo portal (Priority: P2)

- [x] T007 [US2] Preserve compatibility with the existing dialog-handling path in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T008 [US2] Document the confirmation assumption and its boundary in `specs/013-finalizacao-emissao-download-pdf/research.md`

## Phase 5: User Story 3 - Encerrar com um PDF baixado de verdade (Priority: P3)

- [x] T009 [US3] Keep the final wait aligned with the existing download-capture contract
- [x] T010 [US3] Validate that the completed recipe now extends through the expected PDF-producing phase

## Phase 6: Polish

- [x] T011 [P] Create the specification quality checklist in `specs/013-finalizacao-emissao-download-pdf/checklists/requirements.md`
- [x] T012 Validate the updated recipe and active feature package with a local build check
