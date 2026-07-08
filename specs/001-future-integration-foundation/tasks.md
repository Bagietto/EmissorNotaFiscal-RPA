# Tasks: Foundation for Future Integrations

**Input**: Design documents from `/specs/001-future-integration-foundation/`  
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/  
**Tests**: No dedicated test tasks were generated because the feature specification did not explicitly require a TDD-first or test-delivery increment.  
**Organization**: Tasks are grouped by user story to enable independent implementation and validation.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (`US1`, `US2`, `US3`)
- Every task includes an exact file path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create the baseline project skeleton and repository artifacts needed by every story.

- [x] T001 Create the root Worker Service project file in `EmissorNotaFiscal.csproj`
- [x] T002 [P] Create the architectural folder structure with placeholder keep files in `Application/.gitkeep`, `Domain/Models/.gitkeep`, `Domain/Interfaces/.gitkeep`, `Infrastructure/Storage/.gitkeep`, `Infrastructure/Automation/.gitkeep`, and `Infrastructure/Email/.gitkeep`
- [x] T003 [P] Create the configuration folder and options placeholder in `Configuration/WorkerScheduleOptions.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish the hosting, configuration, and observability foundation that blocks all user stories.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T004 Implement typed schedule configuration and validation in `Configuration/WorkerScheduleOptions.cs`
- [x] T005 Create the baseline application configuration in `appsettings.json`
- [x] T006 Implement host bootstrap, `appsettings.json` loading, options binding/validation, DI registration, and infrastructure registration comments in `Program.cs`
- [x] T007 Create the orchestration seam in `Application/FaturamentoOrchestrator.cs` as a behavior-light application boundary
- [x] T008 Create the background worker shell with cancellation-aware looping, explicit interval use, configuration-failure handling, startup/execution logs, and reserved metrics/tracing seams in `Worker.cs`

**Checkpoint**: Foundation ready - user story work can proceed in priority order.

---

## Phase 3: User Story 1 - Establish a stable project skeleton (Priority: P1) 🎯 MVP

**Goal**: Deliver a compilable repository structure that clearly separates application, domain, and infrastructure responsibilities.

**Independent Test**: A maintainer can inspect the repository and verify that the .NET Worker project, architectural folders, and orchestration entry point exist in the planned locations without any future integration logic present.

### Implementation for User Story 1

- [x] T009 [US1] Add project item inclusion and package references that preserve the planned folder structure in `EmissorNotaFiscal.csproj`
- [x] T010 [P] [US1] Add a structural domain model placeholder comment file in `Domain/Models/README.md` describing the intentionally minimal reservation scope
- [x] T011 [P] [US1] Add a structural domain interface placeholder comment file in `Domain/Interfaces/README.md` describing the intentionally minimal reservation scope
- [x] T012 [US1] Refine `Application/FaturamentoOrchestrator.cs` so it documents the future invoice orchestration boundary without adding business logic

**Checkpoint**: User Story 1 is complete when the repository layout alone communicates where future responsibilities belong.

---

## Phase 4: User Story 2 - Start the service with predictable initialization (Priority: P2)

**Goal**: Deliver a startup path that validates required configuration, registers the worker, and runs a safe recurring execution loop.

**Independent Test**: A developer can restore/build/run the project and verify that startup wiring is centralized, the worker is hosted by the default builder, and invalid interval configuration fails clearly before normal execution proceeds.

### Implementation for User Story 2

- [x] T013 [US2] Complete the explicit schedule binding and startup validation flow in `Program.cs`, rejecting missing, empty, zero, negative, or non-convertible interval values
- [x] T014 [US2] Implement the cancellation-aware recurring execution behavior in `Worker.cs`, including the documented operator-restart recovery expectation after configuration failure
- [x] T015 [US2] Update `appsettings.json` with the required explicit interval configuration expected by the worker

**Checkpoint**: User Story 2 is complete when startup and scheduling behavior can be understood and executed independently of future integrations.

---

## Phase 5: User Story 3 - Preserve decoupling for future integrations (Priority: P3)

**Goal**: Deliver clear extension seams for storage, automation, and email integration without introducing concrete adapter implementations.

**Independent Test**: A developer can inspect startup and infrastructure boundaries and identify exactly where future storage, automation, and email services will be registered, while confirming that no placeholder adapters or integration logic were added.

### Implementation for User Story 3

- [x] T016 [P] [US3] Add infrastructure boundary guidance files in `Infrastructure/Storage/README.md`, `Infrastructure/Automation/README.md`, and `Infrastructure/Email/README.md` describing the allowed future adapter category for each folder
- [x] T017 [US3] Refine the DI extension comments and observability seams for future infrastructure adapters in `Program.cs`, including reserved metric names and trace activity names
- [x] T018 [US3] Refine `Worker.cs` and `Application/FaturamentoOrchestrator.cs` to preserve integration-safe boundaries with no direct dependency on future adapter implementations

**Checkpoint**: User Story 3 is complete when future integration points are explicit and the application layer remains decoupled from concrete infrastructure.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finish buildability, documentation alignment, and final validation across all stories.

- [x] T019 [P] Update the implementation guidance to match the final code layout in `specs/001-future-integration-foundation/quickstart.md`
- [x] T020 Validate the end-to-end baseline by running the documented build flow and align any final project metadata in `EmissorNotaFiscal.csproj`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1: Setup**: No dependencies
- **Phase 2: Foundational**: Depends on Phase 1 and blocks all user stories
- **Phase 3: US1**: Depends on Phase 2
- **Phase 4: US2**: Depends on Phase 2 and benefits from US1 structure being in place
- **Phase 5: US3**: Depends on Phase 2 and should be applied after the startup/orchestrator seams exist
- **Phase 6: Polish**: Depends on all selected user stories being complete

### User Story Dependencies

- **US1 (P1)**: No dependency on other user stories after Foundation
- **US2 (P2)**: Uses the shared foundation and the orchestration seam created earlier; does not require US3
- **US3 (P3)**: Uses the startup and worker seams from US2 to finalize decoupled extension points

### Within Each User Story

- Structural files before boundary refinements
- Configuration registration before recurring execution behavior
- Worker/orchestrator boundary refinements after the baseline host and folders exist

### Parallel Opportunities

- `T002` and `T003` can run in parallel after `T001`
- `T010` and `T011` can run in parallel during US1
- `T016` can run independently from `T017` once foundational files exist
- `T019` can start once the final implementation shape is stable, while `T020` is reserved for the final validation pass

---

## Parallel Example: User Story 1

```bash
Task: "Add structural domain model placeholder comment file in Domain/Models/README.md"
Task: "Add structural domain interface placeholder comment file in Domain/Interfaces/README.md"
```

## Parallel Example: User Story 3

```bash
Task: "Add infrastructure boundary guidance files in Infrastructure/Storage/README.md, Infrastructure/Automation/README.md, and Infrastructure/Email/README.md"
Task: "Refine the DI extension comments and observability seams for future infrastructure adapters in Program.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate that the repository structure and orchestration seam are clear and compilable

### Incremental Delivery

1. Setup + Foundational create the runnable Worker baseline
2. US1 makes the structure explicit and reviewable
3. US2 makes startup and scheduling behavior reliable
4. US3 finalizes decoupled extension seams for future integrations
5. Polish aligns docs and final build validation

### Parallel Team Strategy

1. One developer handles project/bootstrap files (`EmissorNotaFiscal.csproj`, `Program.cs`, `appsettings.json`)
2. One developer handles worker/orchestrator seams (`Worker.cs`, `Application/FaturamentoOrchestrator.cs`)
3. Once the foundation is merged, another developer can document the reserved boundaries in `Domain/*/README.md` and `Infrastructure/*/README.md`

---

## Notes

- All tasks follow the required checklist format with task ID and exact file paths.
- No test tasks were included because the current feature scope did not explicitly request them.
- The suggested MVP scope is **Phase 1 + Phase 2 + Phase 3 (US1)**.
- Accepted structural placeholders in this increment are limited to directories, `.gitkeep` files, and descriptive `README.md` files.
