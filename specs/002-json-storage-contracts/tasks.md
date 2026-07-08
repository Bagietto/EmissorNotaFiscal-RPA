# Tasks: Async JSON Contract Storage

**Input**: Design documents from `/specs/002-json-storage-contracts/`  
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/  
**Tests**: No dedicated test tasks were generated because the feature specification did not explicitly require a TDD-first or test-delivery increment.  
**Organization**: Tasks are grouped by user story to enable independent implementation and validation.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this belongs to (`US1`, `US2`, `US3`)
- Every task includes an exact file path

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Create the concrete folder structure and source-file entry points required by this feature.

- [x] T001 Create the billing model folder and source files in `Domain/Models/Faturamento/ConfigFaturamento.cs`, `Domain/Models/Faturamento/ConfigEmissor.cs`, and `Domain/Models/Faturamento/ItemNota.cs`
- [x] T002 Create the automation model folder and source files in `Domain/Models/Automation/FluxoAutomacaoContrato.cs`, `Domain/Models/Automation/EtapaExecucao.cs`, `Domain/Models/Automation/AcaoPasso.cs`, and `Domain/Models/Automation/TipoAcao.cs`
- [x] T003 [P] Create the repository abstraction source file in `Domain/Interfaces/IConfigRepository.cs`
- [x] T004 [P] Create the storage implementation source file in `Infrastructure/Storage/JsonConfigRepository.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish shared contract and storage decisions that every user story depends on.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T005 Implement shared namespace and JSON mapping conventions across all new domain contract files in `Domain/Models/Faturamento/ConfigFaturamento.cs`, `Domain/Models/Faturamento/ConfigEmissor.cs`, `Domain/Models/Faturamento/ItemNota.cs`, `Domain/Models/Automation/FluxoAutomacaoContrato.cs`, `Domain/Models/Automation/EtapaExecucao.cs`, and `Domain/Models/Automation/AcaoPasso.cs`
- [x] T006 Implement the closed automation action enum with the exact ordinal option set in `Domain/Models/Automation/TipoAcao.cs`
- [x] T007 Define the asynchronous repository contract signatures in `Domain/Interfaces/IConfigRepository.cs`
- [x] T008 Implement shared `JsonSerializerOptions`, async stream helpers, and storage-focused exception translation in `Infrastructure/Storage/JsonConfigRepository.cs`

**Checkpoint**: Shared contract and storage foundation is ready for story implementation.

---

## Phase 3: User Story 1 - Load billing configuration contracts (Priority: P1) 🎯 MVP

**Goal**: Deliver strongly typed billing configuration models plus repository loading behavior for local JSON billing files.

**Independent Test**: A maintainer can load a representative billing JSON file through the repository and receive a typed `ConfigFaturamento` object graph, while a missing file returns an empty but valid aggregate.

### Implementation for User Story 1

- [x] T009 [P] [US1] Implement the issuer settings contract mapping in `Domain/Models/Faturamento/ConfigEmissor.cs`
- [x] T010 [P] [US1] Implement the scheduled invoice item contract mapping in `Domain/Models/Faturamento/ItemNota.cs`
- [x] T011 [US1] Implement the root billing configuration aggregate with default-safe collection behavior in `Domain/Models/Faturamento/ConfigFaturamento.cs`
- [x] T012 [US1] Implement asynchronous billing configuration deserialization with missing-file bootstrap behavior in `Infrastructure/Storage/JsonConfigRepository.cs`

**Checkpoint**: User Story 1 is complete when billing configuration files can be loaded safely into typed models without manual JSON traversal.

---

## Phase 4: User Story 2 - Load automation recipe contracts (Priority: P2)

**Goal**: Deliver typed automation recipe models plus repository loading behavior for local JSON automation recipe files.

**Independent Test**: A developer can load a representative automation recipe file through the repository and receive ordered typed stages and actions, while missing or invalid recipes fail descriptively.

### Implementation for User Story 2

- [x] T013 [P] [US2] Implement the automation action contract mapping with nullable dynamic value behavior in `Domain/Models/Automation/AcaoPasso.cs`
- [x] T014 [P] [US2] Implement the execution stage contract mapping in `Domain/Models/Automation/EtapaExecucao.cs`
- [x] T015 [US2] Implement the root automation recipe aggregate in `Domain/Models/Automation/FluxoAutomacaoContrato.cs`
- [x] T016 [US2] Implement asynchronous automation recipe deserialization with missing-file and unsupported-action failure handling in `Infrastructure/Storage/JsonConfigRepository.cs`

**Checkpoint**: User Story 2 is complete when automation recipe files load into typed contracts and invalid operational inputs fail clearly.

---

## Phase 5: User Story 3 - Persist billing configuration updates (Priority: P3)

**Goal**: Deliver predictable asynchronous persistence for billing configuration updates using the same repository abstraction.

**Independent Test**: A maintainer can save a populated billing configuration object to disk and reload the same file without losing issuer or invoice-item data.

### Implementation for User Story 3

- [x] T017 [US3] Implement asynchronous billing configuration save behavior with indented JSON output in `Infrastructure/Storage/JsonConfigRepository.cs`
- [x] T018 [US3] Implement directory creation and write-failure translation for billing configuration persistence in `Infrastructure/Storage/JsonConfigRepository.cs`
- [x] T019 [US3] Align the repository abstraction comments or XML documentation with the final load/save behavior in `Domain/Interfaces/IConfigRepository.cs`

**Checkpoint**: User Story 3 is complete when billing configuration data round-trips through the repository with predictable file output.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finalize feature-level consistency and end-to-end validation across all stories.

- [x] T020 [P] Remove or update now-obsolete placeholder markers related to the new concrete model areas in `Domain/Models/README.md` and `Domain/Interfaces/README.md`
- [x] T021 Validate the documented quickstart flow and align any final project metadata or source-file inclusions in `EmissorNotaFiscal.csproj` and `specs/002-json-storage-contracts/quickstart.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1: Setup**: No dependencies
- **Phase 2: Foundational**: Depends on Phase 1 and blocks all user stories
- **Phase 3: US1**: Depends on Phase 2
- **Phase 4: US2**: Depends on Phase 2 and can proceed independently of US1 after the foundation is complete
- **Phase 5: US3**: Depends on Phase 2 and benefits from the billing models created in US1
- **Phase 6: Polish**: Depends on all selected user stories being complete

### User Story Dependencies

- **US1 (P1)**: No dependency on other user stories after the foundational phase
- **US2 (P2)**: No dependency on US1 behavior, but shares the repository implementation foundation
- **US3 (P3)**: Depends on the billing contract shape established in US1 because it persists the same aggregate

### Within Each User Story

- Billing leaf models before the billing root aggregate
- Automation leaf models before the automation root aggregate
- Contract models before repository behavior that consumes them
- Load behavior before save-roundtrip validation

### Parallel Opportunities

- `T003` and `T004` can run in parallel during setup
- `T009` and `T010` can run in parallel during US1
- `T013` and `T014` can run in parallel during US2
- `T020` can run in parallel with the final validation task once implementation is stable

---

## Parallel Example: User Story 1

```bash
Task: "Implement the issuer settings contract mapping in Domain/Models/Faturamento/ConfigEmissor.cs"
Task: "Implement the scheduled invoice item contract mapping in Domain/Models/Faturamento/ItemNota.cs"
```

## Parallel Example: User Story 2

```bash
Task: "Implement the automation action contract mapping with nullable dynamic value behavior in Domain/Models/Automation/AcaoPasso.cs"
Task: "Implement the execution stage contract mapping in Domain/Models/Automation/EtapaExecucao.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational
3. Complete Phase 3: User Story 1
4. Validate billing configuration loading independently before moving on

### Incremental Delivery

1. Setup + Foundational establish the shared contract and repository base
2. US1 delivers billing configuration loading
3. US2 adds automation recipe loading
4. US3 adds billing persistence and round-trip behavior
5. Polish finalizes documentation and end-to-end validation

### Parallel Team Strategy

1. One developer can handle billing models and billing repository behavior
2. One developer can handle automation models and automation repository behavior after the foundational phase
3. Final polish can be handled after storage behavior stabilizes across both contract groups

---

## Notes

- All tasks follow the required checklist format with task ID and exact file paths.
- No explicit test tasks were included because the current feature scope did not require tests-first delivery.
- The suggested MVP scope is **Phase 1 + Phase 2 + Phase 3 (US1)**.
- The repository implementation in `Infrastructure/Storage/JsonConfigRepository.cs` is intentionally shared across stories, so later story tasks refine the same file rather than creating duplicate storage classes.
