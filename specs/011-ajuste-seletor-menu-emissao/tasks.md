# Tasks: Ajuste de Encoding e Seletor do Menu de Emissao

**Input**: Design documents from `/specs/011-ajuste-seletor-menu-emissao/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md

## Phase 1: Setup

- [x] T001 Review the current emission-menu selector in `receita_paulistana.json`
- [x] T002 Review the authenticated-flow context and confirm the engine does not need selector-specific logic in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

## Phase 2: Foundational

- [x] T003 Confirm the next sequential feature package and update active pointers in `.specify/feature.json` and `AGENTS.md`
- [x] T004 Consolidate the encoding-related investigation context into the new spec artifacts under `specs/011-ajuste-seletor-menu-emissao/`

## Phase 3: User Story 1 - Acessar o menu lateral de emissao com confiabilidade (Priority: P1)

- [x] T005 [US1] Replace the text-based emission-menu selector in `receita_paulistana.json`
- [x] T006 [US1] Target the authenticated emission link through structural DOM evidence instead of visible-label matching in `receita_paulistana.json`

## Phase 4: User Story 2 - Resistir a variacoes de encoding do portal (Priority: P2)

- [x] T007 [US2] Record the `iso-8859-1` resilience rationale in `specs/011-ajuste-seletor-menu-emissao/research.md`
- [x] T008 [US2] Preserve the scope boundary that avoids engine-level encoding logic in `specs/011-ajuste-seletor-menu-emissao/spec.md`

## Phase 5: User Story 3 - Preservar a transicao para o preenchimento do tomador (Priority: P3)

- [x] T009 [US3] Keep the downstream tomador flow assumptions in `specs/011-ajuste-seletor-menu-emissao/quickstart.md`
- [x] T010 [US3] Leave `Infrastructure/Automation/ContractBasedAutomationEngine.cs` unchanged unless runtime evidence requires more

## Phase 6: Polish

- [x] T011 [P] Create the specification quality checklist in `specs/011-ajuste-seletor-menu-emissao/checklists/requirements.md`
- [x] T012 Validate the updated recipe and feature package with a local build check
