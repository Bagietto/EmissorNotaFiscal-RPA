# Contract: JSON Config Repository

## Purpose

Define the expected behavior of the asynchronous repository responsible for loading and saving local JSON contract files for billing configuration and automation recipes.

## Operations

### `ObterConfiguracaoFaturamentoAsync(caminhoArquivo)`

- **Input**: Path to a local JSON file representing the billing configuration contract.
- **Success Output**: One `ConfigFaturamento` aggregate populated from disk.
- **Special Missing-File Behavior**: If the file does not exist, return a safe empty `ConfigFaturamento` aggregate with a non-null issuer object and an empty invoice-item collection.
- **Failure Behavior**:
  - Raise a descriptive storage error if the file exists but cannot be interpreted as the billing configuration contract.
  - Raise a descriptive storage error if the file cannot be accessed for reasons other than absence.

### `SalvarConfiguracaoFaturamentoAsync(caminhoArquivo, dados)`

- **Input**: Path to the target billing configuration file and one populated `ConfigFaturamento` aggregate.
- **Success Output**: The billing configuration is persisted to disk as human-readable JSON.
- **Write Behavior**:
  - The target directory must be created if it does not yet exist.
  - The persisted structure must preserve the billing JSON contract names.
- **Failure Behavior**:
  - Raise a descriptive storage error if the target cannot be written.

### `ObterReceitaAutomacaoAsync(caminhoArquivo)`

- **Input**: Path to a local JSON file representing the automation recipe contract.
- **Success Output**: One `FluxoAutomacaoContrato` aggregate populated from disk.
- **Missing-File Behavior**: If the file does not exist, raise a descriptive storage error indicating that the automation recipe is required.
- **Failure Behavior**:
  - Raise a descriptive storage error when JSON content is malformed.
  - Raise a descriptive storage error when action values cannot be mapped to the supported action set.

## Serialization Expectations

- Reads are case-insensitive with respect to JSON property names.
- Writes use indented formatting for operator readability.
- Contract mapping must remain explicit enough to preserve the external JSON schema expected by maintainers.

## Non-Goals

- Executing automation steps.
- Validating invoice business rules.
- Supporting remote storage, databases, or external configuration providers.
- Introducing dependency injection or runtime orchestration changes beyond the repository contract itself.
