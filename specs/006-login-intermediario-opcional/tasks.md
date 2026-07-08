# Tasks: Suporte a Login Intermediario Opcional

**Input**: Design documents from `/specs/006-login-intermediario-opcional/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md

**Tests**: No dedicated test tasks were requested for this feature increment.

**Organization**: Tasks are grouped by user story to keep the optional-login behavior independently implementable and verifiable.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (`US1`, `US2`, `US3`)
- Each task includes an exact file path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the existing automation recipe and engine files for the optional login branch.

- [x] T001 Review the current optional-login branch and execution flow in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T002 Review the current login recipe structure in `receita_paulistana.json`
- [x] T003 Review the current automation contract types in `Domain/Models/Automation/TipoAcao.cs`, `Domain/Models/Automation/AcaoPasso.cs`, and `Domain/Models/Automation/EtapaExecucao.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish the shared contract and recipe support required before any user story can be completed.

**CRITICAL**: No user story work can begin until this phase is complete

- [x] T004 Add recipe support for an optional login action in `Domain/Models/Automation/TipoAcao.cs`
- [x] T005 Add optional-step metadata needed by the login recipe in `Domain/Models/Automation/AcaoPasso.cs`
- [x] T006 Update the recipe contract shape so the automation engine can distinguish optional and mandatory login steps in `Domain/Models/Automation/EtapaExecucao.cs`
- [x] T007 Update the login recipe to represent the `Login unico` branch as optional in `receita_paulistana.json`

**Checkpoint**: The automation recipe can now express an optional intermediate login path without breaking the existing contract load.

---

## Phase 3: User Story 1 - Entrar com ou sem tela intermediaria (Priority: P1) 🎯 MVP

**Goal**: Allow the login flow to continue whether the intermediate `Login unico` screen appears or not.

**Independent Test**: Run the worker once with the intermediate screen present and once without it, and confirm both executions reach the standard credential screen.

### Implementation for User Story 1

- [x] T008 [US1] Implement optional-step evaluation and skip logic in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T009 [US1] Add selector-presence handling for the optional login screen in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T010 [US1] Keep the standard credential steps executing after the optional branch decision in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

**Checkpoint**: The login flow completes successfully whether the intermediate screen is present or absent.

---

## Phase 4: User Story 2 - Registrar o caminho seguido (Priority: P2)

**Goal**: Make the chosen login path visible in operational logs for each execution.

**Independent Test**: Inspect logs from one run with the intermediate screen and one run without it, and confirm the chosen path is explicit in both cases.

### Implementation for User Story 2

- [x] T011 [US2] Add logging for the optional-login path selection in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T012 [US2] Add logging for skipped optional-login execution in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

**Checkpoint**: The operator can identify the taken path from logs alone.

---

## Phase 5: User Story 3 - Preservar o fluxo principal apos a decisao (Priority: P3)

**Goal**: Keep real portal failures visible after the optional-login decision point.

**Independent Test**: Trigger a downstream failure after the optional branch and confirm the engine still surfaces the real error with step context.

### Implementation for User Story 3

- [x] T013 [US3] Preserve downstream failure propagation after the optional login decision in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T014 [US3] Add explicit error context for failures that occur after the optional branch in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

**Checkpoint**: Failures after the optional path remain attributable to the real failing step.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finalize the implementation guidance and validate the feature end to end.

- [x] T015 [P] Update the implementation guidance to reflect the final optional-login behavior in `specs/006-login-intermediario-opcional/quickstart.md`
- [x] T016 Validate the integrated optional-login flow against the project plan in `specs/006-login-intermediario-opcional/plan.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies
- **Foundational (Phase 2)**: Depends on Setup completion and blocks all user stories
- **User Stories (Phase 3+)**: Depend on Foundational completion
- **Polish (Final Phase)**: Depends on all user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational phase completion
- **User Story 2 (P2)**: Depends on the optional-path behavior being available from US1
- **User Story 3 (P3)**: Depends on the optional-path behavior and logging context established in US1/US2

### Within Each User Story

- Contract and recipe changes before engine behavior
- Optional-path decision before logging the result
- Logging before final polish

### Parallel Opportunities

- `T001`, `T002`, and `T003` can run in parallel
- `T011` and `T012` can run in parallel
- `T015` can run in parallel with final validation once the implementation is stable

---

## Parallel Example: User Story 1

```text
Task: "Implement optional-step evaluation and skip logic in Infrastructure/Automation/ContractBasedAutomationEngine.cs"
Task: "Add selector-presence handling for the optional login screen in Infrastructure/Automation/ContractBasedAutomationEngine.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate that the login flow continues with and without the intermediate screen

### Incremental Delivery

1. Add the recipe support for the optional branch
2. Implement the optional login behavior in the automation engine
3. Add logs that show the selected path
4. Preserve downstream failure visibility
5. Update the quickstart guidance and validate the integrated flow

### Parallel Team Strategy

1. One developer updates the recipe contract and JSON branch
2. Another developer implements the optional engine behavior and logging
3. Final validation can happen once both pieces are merged
