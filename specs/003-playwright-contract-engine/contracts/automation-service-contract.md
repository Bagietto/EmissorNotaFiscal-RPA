# Contract: Automation Service

## Purpose

Define the expected behavior of the asynchronous automation service responsible for executing a metadata-driven browser flow and returning the downloaded PDF path.

## Operation

### `ExecutarFluxoContratoAsync(contrato, dicionarioDadosReais)`

- **Input**:
  - One `FluxoAutomacaoContrato` describing the automation flow.
  - One dictionary of runtime values keyed by the contract's dynamic value references.
- **Success Output**:
  - One non-empty string containing the physical path of the downloaded PDF file produced during the run.
- **Execution Behavior**:
  - Sort contract stages by their declared order before execution.
  - Execute actions sequentially within each stage.
  - Dispatch behavior generically from the declared action type rather than portal-specific branching.
  - Resolve dynamic values from the provided runtime dictionary for input actions.
  - Handle requested native browser dialogs automatically when the contract asks for it.
- **Failure Behavior**:
  - Stop execution immediately when one action fails.
  - Raise an automation-specific error containing the failed stage and action context.
  - Clean up browser resources even when execution fails.

## Runtime Rules

- Missing required runtime dictionary values are treated as execution failures.
- A successful run is not complete unless a concrete PDF path is available.
- Browser visibility mode must be configurable without changing the contract shape.

## Non-Goals

- Hardcoding selectors or flows for any specific external portal.
- Post-processing PDF contents after download.
- Persisting execution history or telemetry to external storage.
- Adding orchestration wiring beyond the service boundary defined by this feature.
