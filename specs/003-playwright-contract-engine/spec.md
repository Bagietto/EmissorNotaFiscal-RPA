# Feature Specification: Contract-Driven Playwright Automation

**Feature Branch**: `003-playwright-contract-engine`  
**Created**: 2026-07-03  
**Status**: Draft  
**Input**: User description: "Criar o contrato abstrato do servico de automacao e o motor interpretador baseado em contratos para executar fluxos dinamicos e retornar o caminho do PDF baixado."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Execute a contract-defined automation flow (Priority: P1)

As a maintainer, I need the service to execute an automation flow described entirely by contract data so that browser-based invoice steps can be updated through metadata instead of rewriting the automation engine for each portal variation.

**Why this priority**: Contract-driven execution is the core value of this feature. Without it, the automation service remains tied to hand-coded workflows and cannot support future portal variations efficiently.

**Independent Test**: Can be fully tested by providing a representative automation contract with ordered stages and actions, then confirming that the service processes the steps in sequence and completes the flow without hardcoded portal logic.

**Acceptance Scenarios**:

1. **Given** a valid automation contract with multiple ordered stages, **When** the automation service executes the contract, **Then** the stages and actions are processed in contract order through one generic execution engine.
2. **Given** a contract containing navigation, input, click, blur, load-wait, and dialog-handling steps, **When** the automation service interprets the actions, **Then** each step is dispatched according to its declared action type rather than a portal-specific code path.

---

### User Story 2 - Inject real execution data into contract steps (Priority: P2)

As a developer, I need dynamic values to be resolved from a real-data dictionary during execution so that the same automation recipe can be reused for different customers and invoice runs.

**Why this priority**: Metadata-driven automation is only reusable if contract steps can be parameterized with real runtime values.

**Independent Test**: Can be fully tested by executing a contract that references dynamic value keys and confirming that input actions use the runtime dictionary values instead of embedded literals.

**Acceptance Scenarios**:

1. **Given** an input action that references a dynamic value key, **When** the automation service executes the step, **Then** the matching runtime value is used for the interaction.
2. **Given** multiple input steps that reference different runtime keys, **When** the automation service processes the contract, **Then** each step uses its own mapped value without cross-step contamination.

---

### User Story 3 - Surface automation outcomes and failures clearly (Priority: P3)

As an orchestrator maintainer, I need the automation service to return the downloaded PDF path on success and fail with explicit step-level context on error so that customer-level failures can be handled predictably.

**Why this priority**: The automation service only becomes operationally useful when success output and failure signals are both actionable for upstream orchestration.

**Independent Test**: Can be fully tested by observing that a successful execution returns one PDF path string and that a failing execution identifies the failed stage and step description before aborting the flow.

**Acceptance Scenarios**:

1. **Given** an automation flow that completes successfully and downloads a PDF, **When** the automation service finishes execution, **Then** it returns the physical path to the downloaded PDF file.
2. **Given** an automation step that times out or cannot find its target element, **When** the automation service executes the step, **Then** the service stops the flow, reports which stage and step failed, and closes browser resources before surfacing the failure.

### Edge Cases

- What happens when the contract has stages out of order? The engine must sort stages by their declared order before execution.
- What happens when an input step references a dynamic value key that is missing from the runtime dictionary? The service must fail explicitly instead of submitting an empty or unintended value silently.
- What happens when the contract includes a dialog-handling step more than once? The engine must continue to behave predictably without duplicating uncontrolled side effects.
- What happens when a step fails in the middle of the contract? The engine must stop further execution, close browser resources, and return a descriptive failure to the caller.
- What happens when no PDF is produced by the end of an otherwise successful run? The service must fail explicitly rather than return an unusable empty path.
- What happens when the browser runs in visual debugging mode? The flow behavior must remain the same apart from browser visibility.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST provide an automation service abstraction that accepts one automation contract plus one runtime dictionary of real values and returns the physical path of the PDF produced by the run.
- **FR-002**: The system MUST provide one contract-driven automation engine implementation that interprets flow metadata generically rather than embedding portal-specific selectors or workflow branches.
- **FR-003**: The system MUST initialize and manage a browser execution lifecycle for the duration of one contract run, including correct cleanup after success or failure.
- **FR-004**: The system MUST support both visual debugging mode and non-visual execution mode through a configurable headless setting.
- **FR-005**: The system MUST sort execution stages by their declared order before dispatching any actions.
- **FR-006**: The system MUST process each action in sequence within its containing stage.
- **FR-007**: The system MUST support contract-driven navigation, text input, button click, blur trigger, load wait, and native dialog handling behaviors.
- **FR-008**: The system MUST resolve runtime input values from the provided real-data dictionary when a contract step references a dynamic value key.
- **FR-009**: The system MUST wait for input targets to become interactable before attempting value entry.
- **FR-010**: The system MUST register automatic handling for native browser dialogs when the contract requests dialog handling.
- **FR-011**: The system MUST wrap each individual action execution in explicit failure handling so the failed stage and step description can be identified.
- **FR-012**: The system MUST stop the contract run immediately when a step fails rather than continuing with downstream actions.
- **FR-013**: The system MUST surface an automation-specific failure to the caller when a step cannot be completed because of timeout, missing selector target, missing runtime value, or other execution error.
- **FR-014**: The system MUST close browser resources even when execution fails mid-flow.
- **FR-015**: The system MUST return a non-empty physical PDF path only when the automation run completes successfully and a PDF has actually been produced.
- **FR-016**: The system MUST remain fully asynchronous across the automation service contract and engine implementation.
- **FR-017**: The system MUST place the automation service abstraction under `Domain/Interfaces` and the contract-driven engine implementation under `Infrastructure/Automation`.

### Key Entities *(include if feature involves data)*

- **Automation Service Request**: The pair of inputs used for one run: one automation contract plus one dictionary of runtime values.
- **Execution Stage**: An ordered unit of automation work containing one or more actions that must be dispatched sequentially.
- **Execution Action**: One contract-defined browser interaction such as navigation, input, click, blur, load wait, or dialog handling.
- **Runtime Data Dictionary**: The map of keys to real values used to populate parameterized contract steps during execution.
- **Automation Run Outcome**: The final result of one contract execution, expressed either as a downloaded PDF path on success or a step-contextualized failure on error.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A maintainer can execute one automation contract containing multiple ordered stages and confirm that the engine follows the declared stage order without editing engine source for portal-specific behavior.
- **SC-002**: A developer can reuse the same automation contract with different runtime dictionaries and observe that input actions use the supplied values rather than embedded fixed values.
- **SC-003**: When an execution succeeds and produces a PDF, the caller receives one concrete file path that can be handed to downstream processing without manual path reconstruction.
- **SC-004**: When an execution step fails, the caller can identify the failed stage and step description from the surfaced error without inspecting low-level browser internals first.
- **SC-005**: A new contributor can inspect the repository and identify within 5 minutes where the generic automation boundary ends and where browser-specific execution begins.

## Assumptions

- The automation contract models created in the previous feature remain the authoritative input shape for this engine.
- The runtime dictionary is the only supported source of dynamic execution values in this increment.
- The browser engine may rely on generic download-path capture behavior, but this feature does not require portal-specific post-processing of the downloaded file.
- A dedicated custom automation exception type is acceptable within this feature because upstream orchestration needs a distinct failure signal.
- Dependency injection registration and orchestration wiring may be handled in a later increment unless needed directly by this feature's implementation scope.
