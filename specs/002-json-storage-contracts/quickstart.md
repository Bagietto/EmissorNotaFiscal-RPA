# Quickstart: Async JSON Contract Storage

## Goal

Verify that the repository contract models and JSON storage implementation align with the feature scope for local asynchronous parsing and persistence.

## Prerequisites

- `.NET 8 SDK` available locally
- Active feature branch `002-json-storage-contracts`
- NuGet restore access for the existing project dependencies

## Implementation Outline

1. Add billing models under `Domain/Models/Faturamento/`.
2. Add automation contracts under `Domain/Models/Automation/`.
3. Add `IConfigRepository` under `Domain/Interfaces/`.
4. Add `JsonConfigRepository` under `Infrastructure/Storage/`.
5. Preserve asynchronous signatures across all repository operations.
6. Keep behavior limited to JSON contract parsing and persistence.

## Suggested Validation Flow

1. Restore and build the project.

```powershell
dotnet restore EmissorNotaFiscal.csproj --ignore-failed-sources
dotnet build EmissorNotaFiscal.csproj --no-restore
```

2. Inspect that the new source files exist only in the expected folders.

```powershell
Get-ChildItem Domain\Models -Recurse
Get-ChildItem Domain\Interfaces -Recurse
Get-ChildItem Infrastructure\Storage -Recurse
```

3. Validate the repository behavior with representative JSON samples in follow-up implementation or tests:
   - Billing config loads into typed objects.
   - Missing billing config returns an empty aggregate.
   - Automation recipe loads into typed stages and actions.
   - Missing automation recipe fails descriptively.
   - Billing config saves and round-trips.

## Expected Outcome

- The repository contains concrete domain contracts for billing and automation JSON documents.
- The repository abstraction is asynchronous and storage-focused.
- The JSON storage implementation is isolated under `Infrastructure/Storage` and does not introduce business or execution logic.
