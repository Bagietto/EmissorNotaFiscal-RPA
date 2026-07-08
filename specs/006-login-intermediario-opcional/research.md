# Research: Suporte a Login Intermediario Opcional

## Decision 1: Represent the optional login as a recipe-level action

- **Decision**: Extend the automation recipe to mark the intermediate login interaction as optional instead of introducing a separate orchestration branch.
- **Rationale**: The feature is about one unstable portal variation, not a general workflow engine. A recipe-level optional action keeps the change local and preserves readability.
- **Alternatives considered**:
  - Separate branching workflow model: rejected because it adds complexity beyond the current need.
  - Hardcoded portal detection in the orchestrator: rejected because it would leak portal logic outside the engine.

## Decision 2: Keep the fallback behavior non-fatal when the optional selector is missing

- **Decision**: Treat the optional login step as a successful no-op when the selector is absent or not visible within a short observation window.
- **Rationale**: The documented requirement is to keep the flow moving in both portal variants without failing the run just because the intermediate screen does not appear.
- **Alternatives considered**:
  - Fail fast on missing selector: rejected because it preserves the current instability.
  - Retry indefinitely until the selector appears: rejected because it can stall the automation and hide real failures.

## Decision 3: Preserve failure propagation after the optional decision point

- **Decision**: Keep any later login or portal failure visible and attributable to the real downstream step that failed.
- **Rationale**: The feature must solve only the optional transition. Masking later issues would reduce diagnosability and create false positives.
- **Alternatives considered**:
  - Swallow downstream errors after optional handling: rejected because it hides real portal regressions.
  - Convert all failures into generic warnings: rejected because it weakens operational support.

## Decision 4: Surface the chosen path in logs

- **Decision**: Emit logs that clearly state whether the optional path was used or skipped.
- **Rationale**: The same recipe can produce two valid paths, so the operator needs explicit traceability for support and troubleshooting.
- **Alternatives considered**:
  - Silent branch selection: rejected because it makes diagnosis slower.
  - Separate external audit store: rejected because it adds infrastructure with no current need.

## Decision 5: Validate through build plus controlled runtime observation

- **Decision**: Use `dotnet build` for structural validation and runtime execution for path verification.
- **Rationale**: There is no dedicated test project in the repository, so the most reliable planning assumption is build-time validation plus targeted execution with logs.
- **Alternatives considered**:
  - Add a new test project as part of this planning stage: rejected because it changes scope and would delay the feature unnecessarily.

