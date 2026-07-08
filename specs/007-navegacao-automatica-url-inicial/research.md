# Research: Navegacao Automatica pela UrlInicial

## Decision 1: Perform bootstrap navigation before the first stage loop

- **Decision**: Navigate to `contrato.UrlInicial` once, before iterating over the recipe stages.
- **Rationale**: The contract root already defines the entry page, so the engine should honor it as the bootstrap state.
- **Alternatives considered**:
  - Keep requiring a manual `Navegar` step: rejected because it duplicates bootstrap knowledge in the recipe.
  - Auto-navigate lazily on the first non-navigation step: rejected because it makes execution order less explicit.

## Decision 2: Keep later `Navegar` actions unchanged

- **Decision**: Leave the existing `Navegar` action behavior intact for mid-flow transitions.
- **Rationale**: The feature only addresses the initial bootstrap, not all navigation behavior.
- **Alternatives considered**:
  - Remove `Navegar` from the recipe model: rejected because it is still needed for later page changes.

## Decision 3: Fail early on missing or invalid initial URL

- **Decision**: Treat an empty or invalid `UrlInicial` as a configuration error when the engine expects a bootstrap page.
- **Rationale**: Silent continuation would preserve the current failure mode and make diagnosis harder.
- **Alternatives considered**:
  - Ignore missing `UrlInicial`: rejected because it can lead to an empty or incorrect page state.

