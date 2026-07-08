# Research: Contract-Driven Playwright Automation

## Decision 1: Encapsulate one full Playwright lifecycle per automation run

- **Decision**: Create and dispose the Playwright instance, browser, browser context, and page within a single asynchronous execution flow for each contract run.
- **Rationale**: The feature needs deterministic cleanup on both success and failure. A per-run lifecycle keeps resource ownership clear and ensures browser state does not leak across customer executions.
- **Alternatives considered**:
  - Shared long-lived browser instance: rejected because it complicates failure isolation and cleanup guarantees.
  - Static page reuse: rejected because it couples runs and makes download/error handling less predictable.

## Decision 2: Use a dedicated download directory and Playwright download capture

- **Decision**: Allow downloads in the browser context, capture the produced download through Playwright's download facilities, and store the file in a controlled local directory so the engine can return a concrete physical path.
- **Rationale**: The engine must return a PDF path without relying on portal-specific logic. Centralized download capture is the cleanest generic mechanism for that requirement.
- **Alternatives considered**:
  - Infer the path from portal page markup: rejected because it would introduce portal-specific assumptions.
  - Require the caller to detect the PDF afterward: rejected because the feature explicitly requires the service itself to return the path.

## Decision 3: Fail explicitly when runtime data keys are missing

- **Decision**: Treat a missing runtime dictionary key required by an input action as an automation failure.
- **Rationale**: Silent fallback to empty values would make debugging difficult and could trigger invalid submissions in external portals.
- **Alternatives considered**:
  - Use empty-string fallback: rejected because it hides configuration mistakes.
  - Skip the step: rejected because it breaks the contract's declared execution semantics.

## Decision 4: Wrap step failures in an automation-specific exception with execution context

- **Decision**: Catch exceptions per action and rethrow a dedicated automation exception containing stage order, stage name, and action description.
- **Rationale**: The orchestrator needs a domain-relevant failure signal, and operators need enough context to identify exactly where the contract run failed.
- **Alternatives considered**:
  - Bubble raw Playwright exceptions: rejected because they do not consistently provide business-relevant execution context.
  - Aggregate all step failures and continue: rejected because the feature requires immediate stop on failure.

## Decision 5: Keep headless behavior configurable at the engine boundary

- **Decision**: Provide a construction-time configuration seam for headless behavior within the automation engine implementation rather than hardcoding visible or hidden browser mode.
- **Rationale**: The feature requires a debug-visual mode without forcing orchestration redesign in the same increment.
- **Alternatives considered**:
  - Always headless: rejected because it prevents visual debugging.
  - Always headed: rejected because it is less practical for unattended runs.
