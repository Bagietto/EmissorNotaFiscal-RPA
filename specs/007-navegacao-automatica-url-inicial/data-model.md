# Data Model: Navegacao Automatica pela UrlInicial

## Entities

### FluxoAutomacaoContrato

Represents the automation contract that now also determines the initial page opened by the engine.

**Attributes**
- `NomeAutomacao` - display name for the flow
- `UrlInicial` - initial page to open before processing recipe stages
- `Etapas` - ordered execution stages

**Rules**
- `UrlInicial` must be available before bootstrap navigation begins.
- `Etapas` remain independent of the bootstrap page open.

### BootstrapDeNavegacao

Represents the initial navigation action performed automatically by the engine.

**Attributes**
- `Url` - the initial URL opened by the engine
- `ExecutadoUmaVez` - indicates the bootstrap occurs only once per run

**Rules**
- Must happen before the first stage is processed.
- Must be logged for traceability.

### AcaoDeNavegacao

Represents an explicit recipe navigation step that remains available for later transitions.

**Attributes**
- `Destino` - target page defined by the recipe
- `OrdemNaEtapa` - position within the stage

**Rules**
- Must not be removed or repurposed by the bootstrap behavior.
- Must continue to work after the automatic initial navigation.

