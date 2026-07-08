# Tasks: Wire Automation Engine

**Input**: Design documents from `/specs/004-wire-automation-engine/`  
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/  
**Tests**: No dedicated test tasks were generated because the feature specification did not explicitly require a TDD-first or test-delivery increment.  
**Organization**: Tasks are grouped by user story to enable independent implementation and validation.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this belongs to (`US1`, `US2`, `US3`)
- Every task includes an exact file path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the existing runtime files for automation wiring work.

- [x] T001 Review and prepare host bootstrap edits in `Program.cs`
- [x] T002 Review and prepare orchestration boundary edits in `Application/FaturamentoOrchestrator.cs`
- [x] T003 [P] Review and prepare automation runtime configuration edits in `appsettings.json`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish the shared registration and configuration foundation required by all user stories.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T004 Register `INfeAutomationService` to `ContractBasedAutomationEngine` in `Program.cs`
- [x] T005 Add explicit `Automation:Headless` and `Automation:DownloadsDirectory` settings in `appsettings.json`
- [x] T006 Align host bootstrap comments and dependency imports for the new automation registration path in `Program.cs`

**Checkpoint**: The host can now resolve the automation abstraction and expose its runtime configuration seam.

---

## Phase 3: User Story 1 - Register the automation engine in the host (Priority: P1) 🎯 MVP

**Goal**: Deliver a host startup path where the automation abstraction resolves to the Playwright engine through normal dependency injection.

**Independent Test**: A maintainer can inspect and run the host startup path, then confirm that the automation service is available through the dependency graph without manual construction.

### Implementation for User Story 1

- [x] T007 [US1] Complete the concrete automation engine service registration flow in `Program.cs`
- [x] T008 [US1] Ensure the registered automation engine can consume host configuration and logging dependencies through `Program.cs`
- [x] T009 [US1] Update startup guidance comments to reflect the now-active automation registration boundary in `Program.cs`

**Checkpoint**: User Story 1 is complete when the automation engine is wired into the host and the registration path is explicit.

---

## Phase 4: User Story 2 - Route orchestration through the automation boundary (Priority: P2)

**Goal**: Deliver an application orchestrator that depends on the automation abstraction instead of remaining a placeholder-only logging boundary.

**Independent Test**: A developer can inspect the orchestration path and confirm that `FaturamentoOrchestrator` receives and uses `INfeAutomationService` without referencing the Playwright implementation directly.

### Implementation for User Story 2

- [x] T010 [US2] Inject `INfeAutomationService` into `Application/FaturamentoOrchestrator.cs`
- [x] T011 [US2] Replace the placeholder-only orchestration behavior with an automation-aware delegation seam in `Application/FaturamentoOrchestrator.cs`
- [x] T012 [US2] Preserve asynchronous error propagation and application-boundary logging around automation delegation in `Application/FaturamentoOrchestrator.cs`

**Checkpoint**: User Story 2 is complete when the worker reaches automation through the orchestrator abstraction path instead of a placeholder-only boundary.

---

## Phase 5: User Story 3 - Expose minimal runtime configuration for automation execution (Priority: P3)

**Goal**: Deliver visible operator-facing configuration for browser visibility and download path behavior through the normal host configuration path.

**Independent Test**: An operator can inspect configuration and startup wiring to confirm that headless mode and download directory behavior are controllable without source-code changes.

### Implementation for User Story 3

- [x] T013 [US3] Expand `appsettings.json` documentation clarity and default values for automation runtime settings in `appsettings.json`
- [x] T014 [US3] Ensure the host configuration pipeline continues to surface automation settings to the registered engine in `Program.cs`
- [x] T015 [US3] Add operator-facing configuration guidance to the application boundary or startup comments in `Program.cs` and `Application/FaturamentoOrchestrator.cs`

**Checkpoint**: User Story 3 is complete when automation visibility and download output are clearly controlled through host configuration.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finalize documentation consistency and validate the end-to-end wiring shape.

- [x] T016 [P] Update the implementation guidance to match the final host/orchestrator wiring in `specs/004-wire-automation-engine/quickstart.md`
- [x] T017 Validate the integrated automation wiring flow and align any final project metadata in `EmissorNotaFiscal.csproj`, `Program.cs`, and `Application/FaturamentoOrchestrator.cs`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1: Setup**: No dependencies
- **Phase 2: Foundational**: Depends on Phase 1 and blocks all user stories
- **Phase 3: US1**: Depends on Phase 2
- **Phase 4: US2**: Depends on Phase 2 and benefits from the host registration created in US1
- **Phase 5: US3**: Depends on Phase 2 and shares the same bootstrap/configuration files as US1
- **Phase 6: Polish**: Depends on all selected user stories being complete

### User Story Dependencies

- **US1 (P1)**: No dependency on other user stories after the foundational phase
- **US2 (P2)**: Depends on the automation abstraction being resolvable from the host
- **US3 (P3)**: Depends on the host registration and configuration path being active

### Within Each User Story

- Host registration before orchestration dependency changes
- Orchestration dependency injection before automation-aware delegation
- Configuration keys before final operator guidance

### Parallel Opportunities

- `T003` can run in parallel with the other setup review tasks
- `T016` can run in parallel with final validation once the code shape is stable

---

## Parallel Example: User Story 1

```bash
Task: "Complete the concrete automation engine service registration flow in Program.cs"
Task: "Update startup guidance comments to reflect the now-active automation registration boundary in Program.cs"
```

## Parallel Example: User Story 3

```bash
Task: "Expand appsettings.json documentation clarity and default values for automation runtime settings in appsettings.json"
Task: "Ensure the host configuration pipeline continues to surface automation settings to the registered engine in Program.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate that the automation engine is resolvable from the host before moving on

### Incremental Delivery

1. Setup + Foundational establish registration and configuration seams
2. US1 wires the automation engine into the host
3. US2 connects the orchestrator to the automation abstraction
4. US3 makes runtime automation settings explicit for operators
5. Polish aligns guidance and validates the integrated flow

### Parallel Team Strategy

1. One developer can focus on `Program.cs` and configuration wiring
2. One developer can focus on the orchestrator dependency update once host registration is stable
3. Final documentation and validation can be completed after the runtime flow is integrated

---

## Notes

- All tasks follow the required checklist format with task ID and exact file paths.
- No explicit test tasks were included because the current feature scope did not require tests-first delivery.
- The suggested MVP scope is **Phase 1 + Phase 2 + Phase 3 (US1)**.
- Most code changes are concentrated in `Program.cs`, `Application/FaturamentoOrchestrator.cs`, and `appsettings.json`, so file-sharing dependencies should be respected during implementation.
