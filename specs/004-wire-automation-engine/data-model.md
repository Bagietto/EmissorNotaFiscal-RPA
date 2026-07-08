# Data Model: Wire Automation Engine

## Automation Host Registration

- **Purpose**: Represents the runtime binding between the automation abstraction and the concrete Playwright engine.
- **Components**:
  - Service abstraction: `INfeAutomationService`
  - Concrete implementation: `ContractBasedAutomationEngine`
- **Behavior notes**:
  - Registration is owned by `Program.cs`.

## Automation Runtime Configuration

- **Purpose**: Host-level settings used by the automation engine during execution.
- **Fields**:
  - `Headless`: boolean control for browser visibility mode.
  - `DownloadsDirectory`: string path control for download output.
- **Behavior notes**:
  - Values are provided through normal application configuration.

## Orchestration Automation Dependency

- **Purpose**: Application-layer dependency that allows `FaturamentoOrchestrator` to delegate browser work through the abstraction boundary.
- **Components**:
  - Orchestrator
  - `INfeAutomationService`
- **Behavior notes**:
  - The orchestrator remains decoupled from Playwright implementation details.

## Worker-to-Orchestrator Execution Path

- **Purpose**: Existing host execution flow that now includes an automation-capable orchestration boundary.
- **Components**:
  - `Worker`
  - `FaturamentoOrchestrator`
  - `INfeAutomationService`
- **Behavior notes**:
  - Asynchronous invocation flow must remain intact after dependency graph updates.
