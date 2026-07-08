# Data Model: Contract-Driven Playwright Automation

## Automation Service Request

- **Purpose**: Represents one automation run request entering the service boundary.
- **Components**:
  - `Contrato`: one `FluxoAutomacaoContrato` aggregate describing the ordered flow.
  - `DicionarioDadosReais`: one dictionary mapping dynamic keys to runtime values.
- **Validation notes**:
  - Runtime values are required whenever contract steps reference dynamic keys.

## FluxoAutomacaoContrato

- **Purpose**: Existing root automation contract reused by this feature.
- **Fields used by the engine**:
  - `NomeAutomacao`
  - `UrlInicial`
  - `Etapas`
- **Behavior notes**:
  - Stages must be sorted by `Ordem` before execution.

## EtapaExecucao

- **Purpose**: Ordered stage of the automation contract.
- **Fields used by the engine**:
  - `Ordem`
  - `NomeEtapa`
  - `Acoes`
- **Behavior notes**:
  - Actions are executed sequentially within the stage.

## AcaoPasso

- **Purpose**: Atomic browser interaction requested by the contract.
- **Fields used by the engine**:
  - `Descricao`
  - `PlaywrightAcao`
  - `SeletorHtml`
  - `ValorDinamicoChave`
- **Behavior notes**:
  - Some action types require selectors.
  - Input actions may require runtime value lookup.

## TipoAcao

- **Purpose**: Closed action vocabulary already defined in the previous feature.
- **Members used by the engine**:
  - `Navegar`
  - `PreencherTexto`
  - `ClicarBotao`
  - `DispararBlur`
  - `AguardarCarregamento`
  - `TratarDialogos`

## Automation Run Outcome

- **Purpose**: Final service result for one contract run.
- **Success state**:
  - One non-empty physical file path to the downloaded PDF.
- **Failure state**:
  - One automation-specific exception carrying stage and action execution context.

## Automation Step Failure

- **Purpose**: Context-rich failure surfaced to the caller when one action cannot be completed.
- **Fields/Context**:
  - Stage order
  - Stage name
  - Action description
  - Underlying execution exception
- **Behavior notes**:
  - Failure stops the flow immediately.
  - Browser resources must still be cleaned up.
