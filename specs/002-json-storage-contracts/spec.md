# Feature Specification: Async JSON Contract Storage

**Feature Branch**: `002-json-storage-contracts`  
**Created**: 2026-07-03  
**Status**: Draft  
**Input**: User description: "Criar os modelos de faturamento e automacao, a interface de repositorio e a implementacao de armazenamento JSON assincrona para leitura e escrita local."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Load billing configuration contracts (Priority: P1)

As a maintainer, I need the billing configuration JSON to map into strongly typed domain objects so that invoice scheduling data can be read safely from disk and consumed by later orchestration flows without manual field parsing.

**Why this priority**: Billing configuration is the primary operational input for the service. Without a stable typed contract, later workflow execution would depend on fragile ad hoc parsing.

**Independent Test**: Can be fully tested by loading a representative billing configuration file and confirming that issuer settings and scheduled invoice items are available through typed properties with no manual dictionary access.

**Acceptance Scenarios**:

1. **Given** a valid billing configuration file containing issuer settings and scheduled invoice items, **When** a maintainer loads it through the repository contract, **Then** the full document is returned as a strongly typed billing configuration object graph.
2. **Given** a billing configuration file path that does not exist, **When** a maintainer requests the billing configuration, **Then** the repository returns a safe empty billing configuration object that can be initialized and saved later without crashing the caller.

---

### User Story 2 - Load automation recipe contracts (Priority: P2)

As a developer, I need the automation recipe JSON to map into typed execution-step contracts so that future browser automation can interpret ordered steps and actions from configuration instead of hardcoded scripts.

**Why this priority**: The automation recipe is the contract that will allow future RPA work to evolve through data-driven execution rather than direct code changes.

**Independent Test**: Can be fully tested by loading a representative automation recipe file and confirming that the automation name, starting URL, ordered stages, and supported action types are available through typed contracts.

**Acceptance Scenarios**:

1. **Given** a valid automation recipe file with ordered stages and actions, **When** a developer loads the recipe through the repository contract, **Then** the recipe is returned with all stages in order and with each action mapped to one supported action type.
2. **Given** an automation recipe file path that does not exist, **When** a developer requests the recipe, **Then** the repository fails with a descriptive domain-facing error that explains the automation contract file is required.

---

### User Story 3 - Persist billing configuration updates (Priority: P3)

As a maintainer, I need billing configuration data to be saved back to disk in a predictable format so that edits made in memory can be persisted safely for later service runs.

**Why this priority**: Once contracts can be loaded, maintainers also need a reliable way to write updated billing configuration without introducing format drift or unsafe file handling.

**Independent Test**: Can be fully tested by saving a populated billing configuration object and confirming that the resulting file is readable, formatted consistently, and can be loaded again through the same repository contract.

**Acceptance Scenarios**:

1. **Given** a populated billing configuration object, **When** a maintainer saves it through the repository contract, **Then** a readable JSON file is written to the requested path using the expected contract structure.
2. **Given** a billing configuration object that was previously saved, **When** the same file is loaded again, **Then** the persisted data round-trips without losing issuer settings or scheduled invoice item fields.

### Edge Cases

- What happens when the billing configuration file does not exist? The repository must return an empty but valid billing configuration structure rather than failing with a low-level file error.
- What happens when the automation recipe file does not exist? The repository must stop with a descriptive domain-facing error because there is no safe default automation flow.
- How does the system handle malformed JSON content? The repository must surface a descriptive failure that makes the invalid file content identifiable to the maintainer.
- How does the system handle unknown or unsupported automation action names? The repository must reject the file rather than silently coercing unsupported actions.
- How does the system handle optional dynamic values for automation actions? The contract must allow the dynamic value key to be absent without invalidating the rest of the action definition.
- How does the system handle empty collections in billing or automation files? The repository must preserve empty lists as valid states when the surrounding contract remains structurally valid.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST provide a billing configuration contract rooted in `ConfigFaturamento` with one issuer settings object and one collection of scheduled invoice items.
- **FR-002**: The system MUST provide an issuer settings contract that carries the provider registration identifier and the configured municipal service code.
- **FR-003**: The system MUST provide an invoice item contract that carries customer identification, contact, billing amount, issuance day, custom description, and last issuance reference.
- **FR-004**: The system MUST provide an automation recipe contract rooted in `FluxoAutomacaoContrato` with an automation name, a starting URL, and an ordered collection of execution stages.
- **FR-005**: The system MUST provide an execution stage contract that preserves stage order, stage name, and the collection of actions belonging to that stage.
- **FR-006**: The system MUST provide an automation action contract that preserves the action description, the supported action type, the target selector, and an optional dynamic value key.
- **FR-007**: The system MUST define exactly six supported automation action types for recipe interpretation: navigate, text input, button click, blur trigger, load wait, and dialog handling.
- **FR-008**: The system MUST provide an abstract repository contract that supports asynchronous retrieval of billing configuration, asynchronous persistence of billing configuration, and asynchronous retrieval of automation recipes.
- **FR-009**: The system MUST provide a file-backed repository implementation that reads and writes the contracts using JSON documents stored on the local file system.
- **FR-010**: The system MUST deserialize repository input in a case-insensitive manner so that property-name casing differences in source JSON do not prevent valid contract loading.
- **FR-011**: The system MUST persist billing configuration output in an indented, human-readable JSON format.
- **FR-012**: The system MUST return a safe empty billing configuration object when the billing configuration file path does not exist.
- **FR-013**: The system MUST raise a descriptive domain-facing error when the automation recipe file path does not exist.
- **FR-014**: The system MUST raise a descriptive domain-facing error when a JSON file cannot be interpreted as one of the supported contracts because of malformed content or unsupported action values.
- **FR-015**: The system MUST keep the new contracts and repository behavior limited to safe disk-to-memory parsing and persistence, without embedding browser automation execution logic or invoice business rules.
- **FR-016**: The system MUST align new source files with the existing architectural boundaries by placing billing models under `Domain/Models/Faturamento`, automation contracts under `Domain/Models/Automation`, repository abstractions under `Domain/Interfaces`, and JSON storage implementation under `Infrastructure/Storage`.
- **FR-017**: The system MUST expose repository operations through asynchronous contracts so that future orchestration layers can compose them without blocking the host workflow.

### Key Entities *(include if feature involves data)*

- **ConfigFaturamento**: The root billing configuration document containing issuer settings and the collection of scheduled invoice items.
- **ConfigEmissor**: The issuer settings portion of the billing configuration that identifies the provider and service code used for issuance.
- **ItemNota**: A scheduled invoice instruction containing customer identifiers, amount, emission timing, descriptive text, and last emission reference.
- **FluxoAutomacaoContrato**: The root automation recipe document containing the automation identity, starting location, and ordered execution stages.
- **EtapaExecucao**: A named stage of the automation recipe that groups ordered actions under one step in the overall flow.
- **AcaoPasso**: An individual action definition inside a stage, including target selector and optional dynamic value binding.
- **TipoAcao**: The closed set of supported action kinds that recipe files may request.
- **ConfigRepository**: The abstraction that loads and saves contract data between local JSON files and the in-memory domain object graph.

### Technical Boundary Notes

- **Async Contract Boundary**: All repository operations are asynchronous and must remain compatible with the project's non-blocking execution model.
- **Storage Boundary**: The repository implementation is limited to local JSON file access and must not add external storage providers in this feature.
- **Automation Boundary**: The automation recipe models describe future executable intent only; they do not execute any browser actions in this feature.
- **Billing Boundary**: The billing models describe configuration shape only; they do not validate tax rules, calculate amounts, or trigger issuance behavior in this feature.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A maintainer can load a representative billing configuration file and access all top-level issuer and invoice-item fields through typed properties without manual JSON traversal.
- **SC-002**: A developer can load a representative automation recipe file and observe all supported action types mapped into the expected closed action set without additional translation rules.
- **SC-003**: A saved billing configuration file can be reloaded through the same repository contract with no loss of issuer fields or scheduled invoice-item fields.
- **SC-004**: Missing billing configuration files and missing automation recipe files result in two distinct, predictable outcomes that a maintainer can explain after one review of the repository contract.
- **SC-005**: A new contributor can identify the correct source location for each billing model, automation contract, repository abstraction, and file-backed repository class within 5 minutes of inspecting the repository tree.

## Assumptions

- Billing configuration is the only contract in this feature that benefits from a safe empty default because maintainers may create or bootstrap it locally before the first save.
- Automation recipes are considered required operational inputs and therefore missing recipe files should fail clearly instead of creating speculative defaults.
- Contract field names will mirror the external JSON documents closely enough that maintainers benefit from a direct mapping-oriented object model.
- The feature is limited to one local JSON-backed repository abstraction and does not yet need multiple repository implementations.
- Validation in this increment is limited to contract readability, supported action values, and missing-file handling rather than full business validation of billing data.
- Later orchestration and automation features will consume these contracts without changing the architectural ownership established here.
