# Research: JSON-Driven Orchestration

## Decision 1: Reuse `IConfigRepository` instead of direct file reads in the orchestrator

- **Decision**: Load `receita_paulistana.json` and `config_notas_v2.json` through the existing `IConfigRepository` abstraction.
- **Rationale**: The repository boundary already centralizes JSON loading behavior and missing-file handling. Reusing it avoids duplicating storage logic in the application layer.
- **Alternatives considered**:
  - Read JSON files directly inside `FaturamentoOrchestrator`: rejected because it breaks the storage abstraction and duplicates parsing concerns.
  - Introduce a second file-loading abstraction: rejected because the existing repository already covers the needed contract shapes.

## Decision 2: Register `IConfigRepository` in the host if not already wired

- **Decision**: Ensure the host resolves `IConfigRepository` to `JsonConfigRepository` through the existing DI path.
- **Rationale**: The orchestrator cannot consume the storage abstraction unless the host can resolve it at runtime.
- **Alternatives considered**:
  - Manual construction inside the orchestrator: rejected because it bypasses DI and couples the application layer to infrastructure.

## Decision 3: Use a deterministic single-item selection rule for the test path

- **Decision**: Select the first available scheduled billing item from `config_notas_v2.json` for this increment's test execution path.
- **Rationale**: The feature explicitly targets one real test execution and not full batch processing. A deterministic first-item rule is simple and predictable.
- **Alternatives considered**:
  - Execute all billing items: rejected because it expands scope to batch orchestration.
  - Select by current day or dynamic rule: rejected because no such rule is required for this first real execution path.

## Decision 4: Source `SenhaWeb` from configuration/environment rather than the billing JSON

- **Decision**: Populate `SenhaWeb` from host configuration or environment variables while other runtime values come from the billing JSON and selected billing item.
- **Rationale**: The existing billing JSON intentionally does not store the web password, but the current automation recipe requires that key for login.
- **Alternatives considered**:
  - Add `SenhaWeb` to the billing JSON in this feature: rejected because the current issue is orchestration, not reshaping the input contracts.
  - Leave `SenhaWeb` absent: rejected because the automation engine will fail on a missing required runtime key.

## Decision 5: Preserve real-execution failure visibility

- **Decision**: Keep orchestration logging around file loading, record selection, dictionary construction, and automation invocation while rethrowing failures.
- **Rationale**: The goal is to replace the placeholder failure with real execution, not to hide downstream problems once the real path is active.
- **Alternatives considered**:
  - Swallow automation failures after logging: rejected because it would make real test results harder to trust.
