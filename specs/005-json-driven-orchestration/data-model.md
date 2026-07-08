# Data Model: JSON-Driven Orchestration

## Automation Recipe File

- **Purpose**: Real JSON contract file consumed by the orchestrator for browser-step execution.
- **Source**: `receita_paulistana.json`
- **Shape reused**:
  - `FluxoAutomacaoContrato`
  - `EtapaExecucao`
  - `AcaoPasso`
- **Behavior notes**:
  - The orchestrator must hand off the loaded contract without reconstructing a placeholder version.

## Billing Configuration File

- **Purpose**: Real JSON billing input consumed by the orchestrator to derive one test execution.
- **Source**: `config_notas_v2.json`
- **Shape reused**:
  - `ConfigFaturamento`
  - `ConfigEmissor`
  - `ItemNota`
- **Behavior notes**:
  - Provides issuer values plus the scheduled billing items available for selection.

## Selected Test Billing Item

- **Purpose**: One deterministic scheduled billing record chosen for the first real execution path.
- **Fields used**:
  - `CnpjCliente`
  - `RazaoSocialPlaceholder`
  - `EmailCliente`
  - `ValorNota`
  - `DescricaoPersonalizada`
- **Behavior notes**:
  - Only one item is selected in this increment.

## Runtime Automation Dictionary

- **Purpose**: Key-value map passed to `INfeAutomationService` for recipe execution.
- **Expected keys in this increment**:
  - `CnpjPrestador`
  - `SenhaWeb`
  - `CnpjCliente`
  - `DescricaoServico`
  - `ValorNota`
- **Behavior notes**:
  - Combines issuer-level data, selected billing-item data, and host-supplied credential data.

## JSON-Driven Orchestration Run

- **Purpose**: One orchestration cycle that loads both JSON files, selects one billing item, builds the runtime dictionary, and invokes automation.
- **Success state**:
  - Automation service is invoked with the real contract and dictionary.
- **Failure state**:
  - Configuration-loading, selection, or automation failures are logged and propagated.
