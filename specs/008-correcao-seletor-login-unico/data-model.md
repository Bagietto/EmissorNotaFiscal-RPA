# Data Model: Correcao do Seletor da Tela de Login Unico

## Entities

### LoginUnicoAction

Represents the optional recipe step that attempts to click the intermediate login button.

**Attributes**
- `Descricao` - label for the action in the recipe
- `PlaywrightAcao` - action type used by the engine
- `SeletorHtml` - selector used to find the button
- `Opcional` - indicates the action can be skipped when absent

**Rules**
- The selector must correspond to the real interactive button element.
- The action must remain optional.

