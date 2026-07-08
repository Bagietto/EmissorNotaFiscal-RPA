# Research: Redirecionamento Browser-Driven do Login Unico

## Decision 1: Wait for the browser navigation triggered by the click

- **Decision**: After the Login Unico action, wait for the browser to finish navigating before continuing.
- **Rationale**: The redirect contains dynamic OAuth data and cannot be replaced by a static URL open.
- **Alternatives considered**:
  - Open a fixed provider URL directly: rejected because it bypasses the real browser flow.
  - Rely only on a generic sleep: rejected because it is brittle and not tied to actual navigation.

## Decision 2: Log the final URL after the redirect

- **Decision**: Emit the final URL reached after the redirect so operators can inspect the observed destination.
- **Rationale**: The investigation showed the destination can change per execution.
- **Alternatives considered**:
  - Omit the final URL log: rejected because it reduces diagnosability.

## Decision 3: Keep the recipe-driven optional click intact

- **Decision**: Preserve the optional Login Unico action and only enhance its post-click handling.
- **Rationale**: The existing recipe already identifies when the button should be clicked; the gap is the browser redirect wait.
- **Alternatives considered**:
  - Replace the optional recipe step with hardcoded redirect logic: rejected because it would duplicate concerns in the engine.

