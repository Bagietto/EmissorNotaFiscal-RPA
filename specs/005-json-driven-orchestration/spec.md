# Feature Specification: JSON-Driven Orchestration

**Feature Branch**: `005-json-driven-orchestration`  
**Created**: 2026-07-03  
**Status**: Draft  
**Input**: User description: "Implementar o FaturamentoOrchestrator para ler config_notas_v2.json e receita_paulistana.json e disparar uma execucao de teste real em vez do contrato placeholder vazio."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Load the real automation contract and billing configuration (Priority: P1)

As a maintainer, I need the orchestrator to load the real JSON configuration files already present in the project so that the runtime flow uses the actual automation recipe and billing data instead of a placeholder contract with no steps.

**Why this priority**: Without consuming the real JSON files, the application cannot move past the current empty-contract failure.

**Independent Test**: Can be fully tested by running the orchestrator and confirming that it reads the automation recipe and billing configuration from the expected project-root files before invoking automation.

**Acceptance Scenarios**:

1. **Given** the JSON configuration files exist in the project root, **When** the orchestrator starts its execution path, **Then** it loads the billing configuration and automation contract from those real files.
2. **Given** the real automation contract contains ordered actions, **When** the orchestrator prepares the execution handoff, **Then** the automation service receives a populated contract rather than an empty placeholder.

---

### User Story 2 - Build a real dynamic-value dictionary for one test execution (Priority: P2)

As a developer, I need the orchestrator to derive the runtime data dictionary from one configured billing item so that the automation service receives the values needed to execute a realistic test run.

**Why this priority**: The automation engine only becomes operational when it receives actual runtime values for the contract keys.

**Independent Test**: Can be fully tested by observing that one billing item is translated into a dictionary containing the expected automation keys used by the recipe.

**Acceptance Scenarios**:

1. **Given** a billing configuration with one or more scheduled invoice items, **When** the orchestrator selects a test execution target, **Then** it builds the expected runtime value map from the selected data.
2. **Given** the automation contract references issuer, customer, description, and value keys, **When** the orchestrator prepares the dictionary, **Then** the automation service receives matching real values for those keys.

---

### User Story 3 - Execute one real orchestration test path and surface the outcome (Priority: P3)

As an operator or maintainer, I need the orchestrator to trigger one real test execution path with the loaded JSON data so that the current runtime no longer stops at the empty-contract branch and instead reports a meaningful automation result or failure.

**Why this priority**: The feature is only useful if the runtime crosses the current boundary and reaches a real automation attempt with real inputs.

**Independent Test**: Can be fully tested by invoking the orchestrator and observing that it attempts one real automation run using the JSON-backed contract and selected billing record, then reports the resulting PDF path or propagated failure.

**Acceptance Scenarios**:

1. **Given** valid JSON files and one selected billing record, **When** the orchestrator executes the test path, **Then** it invokes the automation service with the real contract and real runtime values.
2. **Given** the automation service succeeds or fails, **When** the orchestrator completes the handoff, **Then** it logs the result and preserves failure visibility to the caller instead of masking it behind the previous placeholder behavior.

### Edge Cases

- What happens when one of the JSON files is missing? The orchestrator must fail clearly at the configuration-loading boundary rather than constructing a speculative fallback.
- What happens when the billing configuration contains no scheduled invoice items? The orchestrator must stop with an explicit explanation instead of attempting an invalid test execution.
- What happens when the selected billing record is missing a value required by the automation contract? The resulting automation failure must remain visible through the orchestration path.
- What happens when the automation contract file loads successfully but contains no executable steps? The orchestrator must not silently claim success.
- What happens when multiple billing items exist? The orchestrator must use one predictable test-selection rule instead of running all items implicitly in this increment.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST update `FaturamentoOrchestrator` to load `receita_paulistana.json` and `config_notas_v2.json` from the project-root execution context.
- **FR-002**: The system MUST use the existing configuration-loading abstractions already available in the codebase where applicable instead of reintroducing placeholder contract construction.
- **FR-003**: The system MUST replace the current empty automation contract handoff with the real automation contract loaded from `receita_paulistana.json`.
- **FR-004**: The system MUST select one predictable billing item from `config_notas_v2.json` for the test execution path in this increment.
- **FR-005**: The system MUST build the runtime dictionary required by the automation engine using real values from the selected billing item plus issuer-level data when needed.
- **FR-006**: The system MUST include runtime keys needed by the current automation recipe, including provider identity, customer identity, service description, and invoice value.
- **FR-007**: The system MUST invoke the automation service with the loaded contract and the constructed runtime dictionary.
- **FR-008**: The system MUST fail clearly when either required JSON file is missing or cannot be loaded for orchestration.
- **FR-009**: The system MUST fail clearly when no billing item is available for the test execution path.
- **FR-010**: The system MUST preserve asynchronous execution and failure propagation from the orchestration boundary through the automation-service invocation.
- **FR-011**: The system MUST log enough information to show which billing item or customer record was selected for the test execution path.
- **FR-012**: The system MUST stop using the placeholder contract path that currently guarantees a no-download failure.

### Key Entities *(include if feature involves data)*

- **Automation Recipe File**: The root JSON contract file describing the ordered Playwright steps to execute.
- **Billing Configuration File**: The root JSON file containing issuer settings and the scheduled invoice items available for orchestration.
- **Selected Test Billing Item**: The one scheduled invoice record chosen for the current real test execution path.
- **Runtime Automation Dictionary**: The key-value map derived from the selected billing item and issuer settings, used to populate the automation recipe at runtime.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A maintainer can run the orchestrator and confirm within one execution that the real JSON files are used instead of the previous placeholder contract path.
- **SC-002**: The automation service receives a populated contract and a populated runtime dictionary derived from one real billing record.
- **SC-003**: The previous empty-contract failure mode is eliminated because the orchestrator no longer constructs an execution with zero steps by default.
- **SC-004**: When the orchestration path fails, the failure remains attributable to real JSON-backed execution rather than the old placeholder boundary.

## Assumptions

- The JSON files already created at the project root remain the authoritative inputs for this feature.
- A single-record test execution path is sufficient for this increment; full batch processing of all configured customers can be implemented later.
- The current automation recipe keys are the required baseline for the runtime dictionary in this increment.
- Email distribution and full invoice lifecycle steps remain outside the scope of this feature.
