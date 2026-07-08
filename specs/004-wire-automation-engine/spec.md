# Feature Specification: Wire Automation Engine

**Feature Branch**: `004-wire-automation-engine`  
**Created**: 2026-07-03  
**Status**: Draft  
**Input**: User description: "Registrar o ContractBasedAutomationEngine no Program.cs, conectar o INfeAutomationService ao FaturamentoOrchestrator e preparar o fluxo de orquestracao para consumir a automacao via dependencia injetada."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Register the automation engine in the host (Priority: P1)

As a maintainer, I need the automation service to be registered in the application host so that the worker and orchestration flow can resolve the engine through dependency injection instead of keeping the automation boundary disconnected.

**Why this priority**: Without host registration, the automation engine exists only as an isolated implementation and cannot participate in the runtime flow.

**Independent Test**: Can be fully tested by starting the host and confirming that the automation service can be resolved through the existing dependency graph without manual construction.

**Acceptance Scenarios**:

1. **Given** the application host starts normally, **When** services are registered, **Then** the automation boundary resolves to the concrete automation engine through dependency injection.
2. **Given** automation-related configuration is present, **When** the host is built, **Then** the automation engine receives the configuration it needs without custom composition outside the host bootstrap path.

---

### User Story 2 - Route orchestration through the automation boundary (Priority: P2)

As a developer, I need the `FaturamentoOrchestrator` to depend on the automation service abstraction so that invoice-processing orchestration can evolve behind the application boundary instead of calling browser automation directly from infrastructure-aware code.

**Why this priority**: The orchestration boundary only becomes meaningful when it coordinates real dependencies through abstractions.

**Independent Test**: Can be fully tested by inspecting and running the orchestration path to confirm that the orchestrator uses the automation service dependency rather than only emitting a placeholder log.

**Acceptance Scenarios**:

1. **Given** the orchestrator is invoked by the worker, **When** it begins an automation-oriented execution path, **Then** it delegates browser execution through the injected automation service abstraction.
2. **Given** the automation service changes in a future increment, **When** the orchestrator remains stable, **Then** the orchestration boundary still depends only on the abstraction rather than the concrete engine.

---

### User Story 3 - Expose minimal runtime configuration for automation execution (Priority: P3)

As an operator, I need the runtime host to expose the minimum automation configuration required for safe execution so that browser visibility and download handling can be controlled predictably across environments.

**Why this priority**: The engine cannot run reliably in different environments unless the host exposes the configuration seam required by the automation boundary.

**Independent Test**: Can be fully tested by reviewing configuration and startup wiring to confirm that headless mode and download directory behavior can be controlled without modifying source code.

**Acceptance Scenarios**:

1. **Given** an operator needs visual debugging, **When** they change the automation visibility setting, **Then** the engine runs with the expected browser visibility mode through normal host configuration.
2. **Given** an operator needs a predictable download location, **When** they configure the download directory, **Then** the automation flow uses that location through the normal application configuration path.

### Edge Cases

- What happens when the host starts without the minimum automation configuration? The runtime must still surface a clear configuration path rather than forcing hardcoded engine defaults hidden from operators.
- What happens when the orchestrator is invoked before a real automation contract source is available? The application boundary must fail or short-circuit predictably rather than bypass the abstraction.
- What happens when the automation service throws an execution error? The orchestrator must receive the failure through the abstraction and preserve the application boundary rather than swallowing it silently.
- What happens when the configured download directory is invalid for the environment? The resulting failure must remain attributable to the automation path rather than appearing as unrelated host startup noise.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST register the automation service abstraction to the concrete automation engine through the existing host dependency injection path.
- **FR-002**: The system MUST keep the registration centralized in the application bootstrap path rather than requiring manual construction of the automation engine.
- **FR-003**: The system MUST allow the automation engine to receive its runtime configuration through the standard host configuration mechanism.
- **FR-004**: The system MUST expose a configurable headless mode setting for automation execution through application configuration.
- **FR-005**: The system MUST expose a configurable download directory setting for automation execution through application configuration.
- **FR-006**: The system MUST update the orchestration boundary so it depends on the automation service abstraction rather than remaining a placeholder-only boundary.
- **FR-007**: The system MUST preserve the clean architecture boundary by keeping the `FaturamentoOrchestrator` dependent on the automation abstraction, not on the Playwright-specific implementation type.
- **FR-008**: The system MUST keep the worker execution path compatible with the updated orchestrator dependency graph.
- **FR-009**: The system MUST preserve asynchronous execution flow across host startup, orchestration invocation, and automation-service delegation.
- **FR-010**: The system MUST surface automation-service failures back through the orchestration path without silently suppressing them.
- **FR-011**: The system MUST document the runtime configuration keys needed for automation visibility and download handling so operators can control them without source-code changes.

### Key Entities *(include if feature involves data)*

- **Automation Host Registration**: The dependency injection mapping that binds the automation abstraction to the concrete engine at runtime.
- **Orchestration Automation Dependency**: The application-boundary dependency that allows `FaturamentoOrchestrator` to delegate execution through the automation service abstraction.
- **Automation Runtime Configuration**: The set of host-level settings controlling browser visibility and download-location behavior.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A maintainer can inspect the host bootstrap path and identify within 5 minutes where the automation abstraction is registered to its runtime implementation.
- **SC-002**: A developer can inspect the orchestrator and confirm that browser automation is reached only through the abstraction boundary rather than direct infrastructure coupling.
- **SC-003**: An operator can change automation visibility and download-location behavior through configuration alone, without editing source code.
- **SC-004**: When the automation service fails during orchestration, the failure is observable from the orchestration path rather than disappearing into logs only.

## Assumptions

- The automation engine created in the previous feature remains the concrete implementation to be wired into the host.
- A minimal orchestration handoff is sufficient in this increment; full invoice workflow composition can remain for later features.
- The configuration keys already used by the automation engine are the ones that should now be surfaced explicitly through application configuration.
- No new external systems are introduced in this increment; the work is limited to host wiring, orchestration integration, and configuration exposure.
