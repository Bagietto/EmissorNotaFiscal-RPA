# Contract: Automation Wiring

## Purpose

Define the expected runtime-wiring behavior for the automation abstraction, the orchestrator, and the host configuration path.

## Host Registration

- The host must register `INfeAutomationService` to the concrete automation engine in the central DI bootstrap path.
- The registration must be visible from `Program.cs` and must not require manual instantiation inside application services.

## Orchestration Boundary

- `FaturamentoOrchestrator` must depend on `INfeAutomationService`, not on the Playwright engine type directly.
- Automation failures surfaced by the service must remain observable from the orchestration path.

## Runtime Configuration

- The host configuration path must expose:
  - one setting controlling headless execution
  - one setting controlling the download output directory
- Operators must be able to change those values without editing source code.

## Non-Goals

- Designing the full invoice execution workflow
- Adding portal-specific contracts or runtime data acquisition sources
- Introducing new external systems or persistence providers
