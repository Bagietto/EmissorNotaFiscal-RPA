# Data Model: Async JSON Contract Storage

## ConfigFaturamento

- **Purpose**: Root billing configuration document loaded from `config_notas_v2.json`.
- **Fields**:
  - `ConfiguracoesEmissor`: `ConfigEmissor`, required root issuer configuration.
  - `AgendamentoNotas`: collection of `ItemNota`, defaults to an empty list when bootstrapped locally.
- **Relationships**:
  - Owns one `ConfigEmissor`.
  - Owns zero or more `ItemNota`.
- **Validation notes**:
  - A missing source file may produce a default empty aggregate.
  - Persisted shape must preserve the external JSON contract keys.

## ConfigEmissor

- **Purpose**: Issuer-level configuration for the billing document root.
- **Fields**:
  - `CnpjPrestador`: string provider identifier.
  - `CodigoServicoPaulistana`: string municipal service code.
- **Validation notes**:
  - No business validation of identifier format is introduced in this feature.

## ItemNota

- **Purpose**: A scheduled invoice instruction for one customer.
- **Fields**:
  - `CnpjCliente`: string customer identifier.
  - `RazaoSocialPlaceholder`: string customer display name placeholder.
  - `EmailCliente`: string billing contact email.
  - `ValorNota`: decimal invoice amount.
  - `DiaEmissao`: integer billing day reference.
  - `DescricaoPersonalizada`: string description content.
  - `UltimaEmissao`: string last issuance reference.
- **Validation notes**:
  - This feature preserves data shape only; it does not enforce tax, date, or email business rules.

## FluxoAutomacaoContrato

- **Purpose**: Root automation recipe document loaded from `receita_paulistana.json`.
- **Fields**:
  - `NomeAutomacao`: string automation identifier.
  - `UrlInicial`: string starting URL.
  - `Etapas`: collection of `EtapaExecucao`.
- **Relationships**:
  - Owns zero or more `EtapaExecucao`.
- **Validation notes**:
  - Missing source file is treated as an error.
  - Invalid action values invalidate the contract.

## EtapaExecucao

- **Purpose**: Named ordered stage within the automation recipe.
- **Fields**:
  - `Ordem`: integer stage order.
  - `NomeEtapa`: string stage label.
  - `Acoes`: collection of `AcaoPasso`.
- **Relationships**:
  - Owns zero or more `AcaoPasso`.
- **Validation notes**:
  - Stage ordering is preserved from the source document; this feature does not reorder stages.

## AcaoPasso

- **Purpose**: Atomic automation step definition inside a stage.
- **Fields**:
  - `Descricao`: string operator-facing action description.
  - `PlaywrightAcao`: `TipoAcao`, required supported action kind.
  - `SeletorHtml`: string HTML selector target.
  - `ValorDinamicoChave`: nullable string dynamic binding key.
- **Validation notes**:
  - `ValorDinamicoChave` may be absent.
  - Unsupported action values must fail contract loading.

## TipoAcao

- **Purpose**: Closed set of supported automation action kinds.
- **Members**:
  - `Navegar`
  - `PreencherTexto`
  - `ClicarBotao`
  - `DispararBlur`
  - `AguardarCarregamento`
  - `TratarDialogos`
- **Validation notes**:
  - The enum is ordinally stable and must expose exactly the six requested values.

## IConfigRepository

- **Purpose**: Application-facing repository abstraction for loading and saving local JSON contracts asynchronously.
- **Operations**:
  - Load billing configuration.
  - Save billing configuration.
  - Load automation recipe.
- **Behavior notes**:
  - Billing load may bootstrap an empty aggregate on missing files.
  - Automation load must fail descriptively on missing files.
  - Invalid content should be translated into storage-focused errors.
