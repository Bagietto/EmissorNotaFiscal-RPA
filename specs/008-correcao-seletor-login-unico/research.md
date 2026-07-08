# Research: Correcao do Seletor da Tela de Login Unico

## Decision 1: Prefer a structural selector over text matching

- **Decision**: Use `button.oauth-button` for the Login Unico step.
- **Rationale**: The real HTML exposes a stable button structure, while the visible text can vary by acentuation or encoding.
- **Alternatives considered**:
  - `text=Login unico`: rejected because it does not match the real rendered text reliably.
  - `span.oauth-name`: rejected because it targets inner text instead of the interactive element.

## Decision 2: Keep the step optional

- **Decision**: Preserve the optional behavior already present in the recipe.
- **Rationale**: The portal does not show the intermediate screen on every run, so the current fallback behavior must remain unchanged.
- **Alternatives considered**:
  - Make the click mandatory: rejected because it would break runs that skip the intermediate screen.

