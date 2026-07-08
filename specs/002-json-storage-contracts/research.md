# Research: Async JSON Contract Storage

## Decision 1: Use `System.Text.Json` with explicit property mapping attributes

- **Decision**: Use `System.Text.Json` as the only serializer and decorate contract properties with explicit JSON name mappings where the external file schema must remain stable.
- **Rationale**: The feature requires safe local JSON parsing only, and the project already targets `.NET 8`, where `System.Text.Json` is the default lightweight option. Explicit mapping avoids ambiguity across snake_case billing fields and the mixed naming expected in automation recipe documents.
- **Alternatives considered**:
  - `Newtonsoft.Json`: rejected because it adds an unnecessary extra dependency for a narrow local-file use case.
  - Naming-policy-only mapping: rejected because explicit contract names are clearer and safer for schema preservation.

## Decision 2: Keep repository operations fully asynchronous with stream-based file access

- **Decision**: All repository operations will use asynchronous file APIs and expose `Task` / `Task<T>` signatures only.
- **Rationale**: The feature explicitly requires asynchronous C# patterns and must remain compatible with the worker-style host flow already established in the repository. Stream-based async reads and writes avoid introducing blocking I/O into future orchestration paths.
- **Alternatives considered**:
  - Synchronous `File.ReadAllText` / `File.WriteAllText`: rejected because it violates the stated async requirement.
  - `ValueTask` return types: rejected because the repository performs real file I/O and does not need the additional complexity at this stage.

## Decision 3: Differentiate missing-file behavior by contract type

- **Decision**: Return a safe empty `ConfigFaturamento` aggregate when the billing configuration file is missing, but throw a descriptive domain-facing exception when the automation recipe file is missing.
- **Rationale**: The billing configuration may need local bootstrap creation before first save, while an automation recipe has no meaningful default flow and should fail loudly if absent.
- **Alternatives considered**:
  - Throw for both missing files: rejected because it makes initial billing configuration bootstrap less practical.
  - Return empty defaults for both file types: rejected because an empty automation flow could mask an operational setup problem.

## Decision 4: Translate malformed or unsupported content into descriptive storage errors

- **Decision**: Catch JSON parsing and enum-conversion failures and rethrow them as descriptive storage-focused exceptions that identify the contract type and source path.
- **Rationale**: The spec requires friendly failure modes. Repository consumers need actionable diagnostics without depending on low-level serializer messages as the sole explanation.
- **Alternatives considered**:
  - Let raw serializer exceptions bubble unchanged: rejected because the resulting errors are less consistent for operators.
  - Swallow invalid data and continue with partial objects: rejected because it would make configuration errors harder to detect and debug.

## Decision 5: Preserve existing project boundaries and avoid new application services

- **Decision**: Limit implementation to domain models, one repository interface, and one storage implementation without adding new application services or startup wiring in this increment.
- **Rationale**: The feature is focused on data contracts and storage parsing. Additional orchestration or DI work would expand scope beyond what the spec requires.
- **Alternatives considered**:
  - Add service registration in `Program.cs`: rejected because the spec does not require wiring the repository into runtime flow yet.
  - Add automation executor placeholders: rejected because the feature explicitly stops at contract parsing and persistence.
