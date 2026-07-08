# Quickstart: Modo Assistido para Caption no PMSP Auth

Verify that the worker can pause for human intervention on a `pmspauth` challenge and resume automatically after the operator resolves it.

## Preconditions

- `appsettings.json` or command-line arguments enable `Automation:AssistedMode:Enabled`
- The existing login flow still reaches `pmspauth`
- The environment allows a visible browser session

## Validation Steps

1. Enable assisted mode and keep a finite `Automation:AssistedMode:HumanInterventionTimeoutMinutes`.
2. Run the worker through the normal login path until `pmspauth` is reached.
3. When a `mcaptcha`-style challenge appears, confirm that the browser stays open and the logs report that human intervention is being awaited.
4. Solve the challenge manually in the visible browser.
5. Confirm that the worker resumes only after the challenge disappears and the flow reaches a valid continuation state.

## Expected Results

- Assisted mode forces a visible browser session.
- The worker does not fail immediately when the challenge appears.
- Successful manual resolution allows the automation to resume on its own.
- Missing manual resolution until the configured deadline ends with a clear timeout message.
