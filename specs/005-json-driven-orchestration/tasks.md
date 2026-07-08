# Tasks: JSON-Driven Orchestration

**Input**: Design documents from `/specs/005-json-driven-orchestration/`  
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/  
**Tests**: No dedicated test tasks were generated because the feature specification did not explicitly require a TDD-first or test-delivery increment.  
**Organization**: Tasks are grouped by user story to enable independent implementation and validation.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this belongs to (`US1`, `US2`, `US3`)
- Every task includes an exact file path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the orchestration and host files for the transition from placeholder execution to JSON-backed execution.

- [x] T001 Review and prepare orchestrator edits in `Application/FaturamentoOrchestrator.cs`
- [x] T002 Review and prepare host registration edits in `Program.cs`
- [x] T003 [P] Review the root input files used by the orchestration path in `receita_paulistana.json` and `config_notas_v2.json`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish the shared runtime dependencies needed before real JSON-backed execution can occur.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T004 Register `IConfigRepository` to `JsonConfigRepository` in `Program.cs`
- [x] T005 Inject `IConfigRepository` and `IConfiguration` into `Application/FaturamentoOrchestrator.cs`
- [x] T006 Add deterministic root-file path resolution helpers for `receita_paulistana.json` and `config_notas_v2.json` in `Application/FaturamentoOrchestrator.cs`

**Checkpoint**: The orchestrator can now access the real JSON inputs through the existing storage abstraction.

---

## Phase 3: User Story 1 - Load the real automation contract and billing configuration (Priority: P1) 🎯 MVP

**Goal**: Deliver a real orchestration path that loads the automation recipe and billing configuration from the project-root JSON files.

**Independent Test**: A maintainer can invoke the orchestrator and confirm that it loads both real JSON files through `IConfigRepository` before calling the automation service.

### Implementation for User Story 1

- [x] T007 [US1] Replace the placeholder contract creation with repository-based loading of `receita_paulistana.json` in `Application/FaturamentoOrchestrator.cs`
- [x] T008 [US1] Load the billing configuration from `config_notas_v2.json` through `IConfigRepository` in `Application/FaturamentoOrchestrator.cs`
- [x] T009 [US1] Add explicit failure handling and logging for missing or invalid orchestration input files in `Application/FaturamentoOrchestrator.cs`

**Checkpoint**: User Story 1 is complete when the automation service receives a real loaded contract instead of the previous empty placeholder.

---

## Phase 4: User Story 2 - Build a real dynamic-value dictionary for one test execution (Priority: P2)

**Goal**: Deliver deterministic selection of one billing record and construct the runtime value map expected by the automation recipe.

**Independent Test**: A developer can inspect or run the orchestrator and confirm that one selected billing record becomes a populated runtime dictionary with the expected recipe keys.

### Implementation for User Story 2

- [x] T010 [US2] Implement deterministic selection of the first scheduled billing item in `Application/FaturamentoOrchestrator.cs`
- [x] T011 [US2] Build the runtime dictionary with `CnpjPrestador`, `SenhaWeb`, `CnpjCliente`, `DescricaoServico`, and `ValorNota` in `Application/FaturamentoOrchestrator.cs`
- [x] T012 [US2] Add explicit failure handling when no billing item exists or when required runtime values cannot be resolved in `Application/FaturamentoOrchestrator.cs`

**Checkpoint**: User Story 2 is complete when one real billing record produces a valid runtime dictionary for the automation engine.

---

## Phase 5: User Story 3 - Execute one real orchestration test path and surface the outcome (Priority: P3)

**Goal**: Deliver a real automation invocation path using the loaded JSON contract and constructed runtime values, with meaningful logging and failure propagation.

**Independent Test**: An operator can invoke the orchestrator and observe that it attempts one JSON-backed automation run, then logs the selected record and the resulting PDF path or failure.

### Implementation for User Story 3

- [x] T013 [US3] Invoke `INfeAutomationService` with the loaded contract and constructed dictionary in `Application/FaturamentoOrchestrator.cs`
- [x] T014 [US3] Log the selected billing record and successful PDF path outcome in `Application/FaturamentoOrchestrator.cs`
- [x] T015 [US3] Preserve downstream automation failure visibility while removing the old placeholder-only execution branch in `Application/FaturamentoOrchestrator.cs`

**Checkpoint**: User Story 3 is complete when the runtime reaches a true JSON-backed automation attempt and reports a real result or propagated failure.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finalize documentation consistency and validate the integrated JSON-driven execution path.

- [x] T016 [P] Update the implementation guidance to match the final JSON-driven orchestration flow in `specs/005-json-driven-orchestration/quickstart.md`
- [x] T017 Validate the integrated orchestration flow and align any final project metadata in `Program.cs`, `Application/FaturamentoOrchestrator.cs`, and `EmissorNotaFiscal.csproj`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1: Setup**: No dependencies
- **Phase 2: Foundational**: Depends on Phase 1 and blocks all user stories
- **Phase 3: US1**: Depends on Phase 2
- **Phase 4: US2**: Depends on Phase 2 and uses the loaded billing configuration from US1
- **Phase 5: US3**: Depends on Phase 2 and benefits from the loaded contract and runtime dictionary produced in US1 and US2
- **Phase 6: Polish**: Depends on all selected user stories being complete

### User Story Dependencies

- **US1 (P1)**: No dependency on other user stories after the foundational phase
- **US2 (P2)**: Depends on the real billing configuration being available from US1
- **US3 (P3)**: Depends on both the real contract from US1 and the runtime dictionary from US2

### Within Each User Story

- Host registration before orchestrator consumption
- Real file loading before billing-item selection
- Billing-item selection before runtime-dictionary construction
- Runtime-dictionary construction before automation invocation

### Parallel Opportunities

- `T003` can run in parallel with the other setup review tasks
- `T016` can run in parallel with final validation once the code shape is stable

---

## Parallel Example: User Story 1

```bash
Task: "Load the billing configuration from config_notas_v2.json through IConfigRepository in Application/FaturamentoOrchestrator.cs"
Task: "Add explicit failure handling and logging for missing or invalid orchestration input files in Application/FaturamentoOrchestrator.cs"
```

## Parallel Example: User Story 3

```bash
Task: "Invoke INfeAutomationService with the loaded contract and constructed dictionary in Application/FaturamentoOrchestrator.cs"
Task: "Log the selected billing record and successful PDF path outcome in Application/FaturamentoOrchestrator.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate that the placeholder contract path has been replaced by real JSON loading before moving on

### Incremental Delivery

1. Setup + Foundational establish repository access and root-file resolution
2. US1 loads the real JSON inputs
3. US2 selects one billing item and builds the runtime dictionary
4. US3 invokes automation with the real inputs and surfaces the result
5. Polish aligns guidance and validates the integrated path

### Parallel Team Strategy

1. One developer can wire `IConfigRepository` into the host while another prepares the orchestrator refactor
2. Once the real JSON loading path is stable, the runtime-dictionary logic can be completed
3. Final automation-invocation behavior and validation can be completed after the dictionary shape is in place

---

## Notes

- All tasks follow the required checklist format with task ID and exact file paths.
- No explicit test tasks were included because the current feature scope did not require tests-first delivery.
- The suggested MVP scope is **Phase 1 + Phase 2 + Phase 3 (US1)**.
- Most code changes are concentrated in `Application/FaturamentoOrchestrator.cs` plus a small registration change in `Program.cs`, so file-sharing dependencies should be respected during implementation.
