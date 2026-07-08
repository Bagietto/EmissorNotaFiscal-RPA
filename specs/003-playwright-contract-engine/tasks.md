# Tasks: Contract-Driven Playwright Automation

**Input**: Design documents from `/specs/003-playwright-contract-engine/`  
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/  
**Tests**: No dedicated test tasks were generated because the feature specification did not explicitly require a TDD-first or test-delivery increment.  
**Organization**: Tasks are grouped by user story to enable independent implementation and validation.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this belongs to (`US1`, `US2`, `US3`)
- Every task includes an exact file path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create the concrete service and automation-engine source file entry points required by this feature.

- [x] T001 Create the automation service abstraction source file in `Domain/Interfaces/INfeAutomationService.cs`
- [x] T002 Create the contract-driven engine source file in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish the shared automation engine scaffolding that every user story depends on.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T003 Define the asynchronous automation service contract in `Domain/Interfaces/INfeAutomationService.cs`
- [x] T004 Implement the browser lifecycle scaffold, headless configuration seam, and Playwright dependency setup in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T005 Implement the shared stage-ordering, sequential action dispatch loop, and per-step failure wrapper structure in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T006 Add a dedicated automation exception type or equivalent contextual failure wrapper inside `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

**Checkpoint**: Shared automation execution foundation is ready for story implementation.

---

## Phase 3: User Story 1 - Execute a contract-defined automation flow (Priority: P1) 🎯 MVP

**Goal**: Deliver a generic Playwright engine that interprets contract-defined stages and action types without portal-specific branching.

**Independent Test**: A maintainer can provide a representative automation contract and confirm that the engine sorts stages by order and dispatches each supported action type through generic Playwright behavior.

### Implementation for User Story 1

- [x] T007 [US1] Implement ordered stage processing and sequential action execution in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T008 [US1] Implement contract-driven handling for `Navegar`, `ClicarBotao`, `DispararBlur`, `AguardarCarregamento`, and `TratarDialogos` in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T009 [US1] Implement reusable Playwright page interaction helpers for selector lookup, visibility waiting, and dialog registration in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

**Checkpoint**: User Story 1 is complete when a generic automation contract can drive a full ordered browser flow without portal-specific code paths.

---

## Phase 4: User Story 2 - Inject real execution data into contract steps (Priority: P2)

**Goal**: Deliver runtime dictionary resolution so parameterized contract steps can be reused across different customer executions.

**Independent Test**: A developer can execute a contract with input actions tied to runtime keys and confirm that each step uses the provided runtime value or fails explicitly when the key is missing.

### Implementation for User Story 2

- [x] T010 [US2] Implement contract-driven `PreencherTexto` behavior with runtime dictionary lookup in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T011 [US2] Implement explicit missing-runtime-key failure handling for parameterized input actions in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T012 [US2] Align the automation service abstraction comments or XML documentation with the final runtime-data behavior in `Domain/Interfaces/INfeAutomationService.cs`

**Checkpoint**: User Story 2 is complete when the same automation contract can be reused safely with different runtime data dictionaries.

---

## Phase 5: User Story 3 - Surface automation outcomes and failures clearly (Priority: P3)

**Goal**: Deliver concrete PDF path return behavior and actionable step-level error signaling for orchestrator consumption.

**Independent Test**: A maintainer can observe that a successful run returns one physical PDF path and that a failed run identifies the failed stage and step while still cleaning up browser resources.

### Implementation for User Story 3

- [x] T013 [US3] Implement generic download capture and physical PDF path resolution in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T014 [US3] Implement final success validation that fails when no usable PDF path is produced in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- [x] T015 [US3] Refine the per-step error handling and disposal flow so browser resources are always closed and the surfaced error includes stage and action context in `Infrastructure/Automation/ContractBasedAutomationEngine.cs`

**Checkpoint**: User Story 3 is complete when automation success returns a usable PDF path and automation failure returns actionable execution context.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finalize documentation consistency and end-to-end validation across all stories.

- [x] T016 [P] Update the automation boundary guidance to reflect the new concrete engine in `Infrastructure/Automation/README.md` and `Domain/Interfaces/README.md`
- [x] T017 Validate the documented quickstart flow and align any final project metadata or source-file inclusions in `EmissorNotaFiscal.csproj` and `specs/003-playwright-contract-engine/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1: Setup**: No dependencies
- **Phase 2: Foundational**: Depends on Phase 1 and blocks all user stories
- **Phase 3: US1**: Depends on Phase 2
- **Phase 4: US2**: Depends on Phase 2 and builds on the dispatch flow from US1
- **Phase 5: US3**: Depends on Phase 2 and benefits from the execution path implemented in US1 and US2
- **Phase 6: Polish**: Depends on all selected user stories being complete

### User Story Dependencies

- **US1 (P1)**: No dependency on other user stories after the foundational phase
- **US2 (P2)**: Depends on the generic dispatch engine from US1 because runtime value injection happens inside action execution
- **US3 (P3)**: Depends on the execution flow from US1 and the runtime input behavior from US2 because success/failure outcomes are observed from completed runs

### Within Each User Story

- Service contract before engine behavior that fulfills it
- Generic action dispatch before runtime-data specialization
- Runtime-data specialization before final PDF outcome handling
- Outcome validation after download capture is in place

### Parallel Opportunities

- `T001` and `T002` can start independently during setup
- `T016` can run in parallel with the final validation task once implementation is stable

---

## Parallel Example: User Story 1

```bash
Task: "Implement contract-driven handling for Navegar, ClicarBotao, DispararBlur, AguardarCarregamento, and TratarDialogos in Infrastructure/Automation/ContractBasedAutomationEngine.cs"
Task: "Implement reusable Playwright page interaction helpers for selector lookup, visibility waiting, and dialog registration in Infrastructure/Automation/ContractBasedAutomationEngine.cs"
```

## Parallel Example: User Story 3

```bash
Task: "Implement generic download capture and physical PDF path resolution in Infrastructure/Automation/ContractBasedAutomationEngine.cs"
Task: "Refine the per-step error handling and disposal flow so browser resources are always closed and the surfaced error includes stage and action context in Infrastructure/Automation/ContractBasedAutomationEngine.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate generic contract execution before adding runtime-data and outcome-specific behavior

### Incremental Delivery

1. Setup + Foundational establish the automation boundary and Playwright engine scaffold
2. US1 delivers generic contract execution
3. US2 adds runtime value injection
4. US3 adds PDF outcome and failure-signal handling
5. Polish finalizes documentation and end-to-end validation

### Parallel Team Strategy

1. One developer can define the service boundary and browser lifecycle scaffold
2. Another developer can refine runtime-data behavior after the generic execution path is stable
3. Final outcome handling and documentation can be completed once download capture behavior is implemented

---

## Notes

- All tasks follow the required checklist format with task ID and exact file paths.
- No explicit test tasks were included because the current feature scope did not require tests-first delivery.
- The suggested MVP scope is **Phase 1 + Phase 2 + Phase 3 (US1)**.
- Most story work converges on `Infrastructure/Automation/ContractBasedAutomationEngine.cs`, so file-sharing dependencies should be respected even when tasks appear conceptually parallel.
