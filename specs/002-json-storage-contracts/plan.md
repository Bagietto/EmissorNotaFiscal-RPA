# Implementation Plan: Async JSON Contract Storage

**Branch**: `002-json-storage-contracts` | **Date**: 2026-07-03 | **Spec**: [spec.md](./spec.md)  
**Input**: Feature specification from `/specs/002-json-storage-contracts/spec.md`

## Summary

Add strongly typed billing and automation contract models, plus an asynchronous JSON-backed repository that reads and writes local files within the existing Clean Architecture boundaries. The implementation will focus on safe disk-to-memory parsing, predictable missing-file behavior, and `.NET 8`-aligned asynchronous repository signatures.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: `System.Text.Json`, `System.Text.Json.Serialization`, existing `Microsoft.Extensions.*` host dependencies already present in the project  
**Storage**: Local file system JSON files only  
**Testing**: `dotnet test` with xUnit in follow-up validation work  
**Target Platform**: Windows-hosted .NET 8 Worker runtime, portable to standard .NET 8 supported environments  
**Project Type**: Single-project Worker Service with layered folders  
**Performance Goals**: Configuration-sized JSON files should load and save without noticeable operator delay during normal local execution  
**Constraints**: All repository APIs must be asynchronous; implementation is limited to local JSON parsing/persistence; no Playwright execution logic; no invoice business rules; missing billing config returns an empty aggregate; missing automation recipe fails descriptively  
**Scale/Scope**: One repository abstraction, one JSON-backed implementation, two domain model groups (`Faturamento` and `Automation`), and one focused storage behavior increment

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

`.specify/memory/constitution.md` remains a placeholder template with no active enforceable principles. Current gate result: PASS with no constitutional restrictions beyond staying within the approved feature scope.

Post-design re-check: PASS. The design remains scoped to domain contracts, asynchronous repository abstractions, and local JSON storage behavior only.

## Project Structure

### Documentation (this feature)

```text
specs/002-json-storage-contracts/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── json-config-repository-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
EmissorNotaFiscal/
├── EmissorNotaFiscal.csproj
├── Application/
│   └── FaturamentoOrchestrator.cs
├── Domain/
│   ├── Interfaces/
│   │   └── IConfigRepository.cs
│   └── Models/
│       ├── Automation/
│       │   ├── AcaoPasso.cs
│       │   ├── EtapaExecucao.cs
│       │   ├── FluxoAutomacaoContrato.cs
│       │   └── TipoAcao.cs
│       └── Faturamento/
│           ├── ConfigEmissor.cs
│           ├── ConfigFaturamento.cs
│           └── ItemNota.cs
└── Infrastructure/
    └── Storage/
        └── JsonConfigRepository.cs
```

**Structure Decision**: Keep the existing single Worker project and extend its reserved Clean Architecture folders with concrete domain contracts and one storage implementation. Domain contracts stay split by business concern (`Faturamento` and `Automation`), while file persistence remains isolated under `Infrastructure/Storage`.

## Phase 0: Research Summary

- Confirm `System.Text.Json` with explicit property-name mapping is the correct fit for mixed naming styles between billing JSON and automation recipe contracts.
- Confirm asynchronous file access should be implemented with stream-based APIs and repository methods returning `Task` / `Task<T>`.
- Confirm missing-file handling should diverge by contract type: billing config gets a safe empty aggregate, while automation recipe produces a descriptive exception because there is no safe default execution flow.
- Confirm unsupported enum values and malformed JSON should be surfaced as descriptive domain-facing storage errors rather than leaking raw parsing context directly to orchestrators.

## Phase 1: Design Summary

- Model billing configuration as a root aggregate with issuer settings plus a collection of invoice schedule items.
- Model automation recipes as a root aggregate with ordered stages and a closed enum-based action vocabulary.
- Use JSON property mapping attributes on domain contracts where external file naming must remain stable and explicit.
- Define `IConfigRepository` as the application-facing abstraction for asynchronous load/save operations.
- Implement `JsonConfigRepository` with shared serializer options, stream-based async reads/writes, directory-aware save behavior, and consistent exception translation for missing or invalid files.

## Complexity Tracking

No constitutional violations or exceptional complexity require justification at this planning stage.
