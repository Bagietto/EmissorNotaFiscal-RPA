# Quickstart: JSON-Driven Orchestration

## Goal

Verify that `FaturamentoOrchestrator` now consumes `receita_paulistana.json` and `config_notas_v2.json` to trigger one real automation execution path instead of handing off an empty placeholder contract.

## Prerequisites

- `.NET 8 SDK` available locally
- Active feature branch `005-json-driven-orchestration`
- `receita_paulistana.json` and `config_notas_v2.json` present at the project root
- Required runtime credentials such as `SenhaWeb` available through environment or application configuration

## Implementation Outline

1. Register `IConfigRepository` in the host so the worker resolves the JSON storage implementation.
2. Update `FaturamentoOrchestrator` to resolve `receita_paulistana.json` and `config_notas_v2.json` from the project root.
3. Load the automation contract and billing configuration through `IConfigRepository`.
4. Select the first scheduled billing item as the deterministic test target.
5. Build the runtime dictionary expected by the automation recipe, including `SenhaWeb` from configuration.
6. Invoke `INfeAutomationService` with the real contract and real values.

## Suggested Validation Flow

1. Restore and build the project.

```powershell
dotnet restore EmissorNotaFiscal.csproj
dotnet build EmissorNotaFiscal.csproj --no-restore
```

2. Inspect the orchestration integration points.

```powershell
Get-Content Program.cs
Get-Content Application\FaturamentoOrchestrator.cs
Get-Content receita_paulistana.json
Get-Content config_notas_v2.json
```

3. Provide the required runtime credential before executing the worker:

```powershell
$env:Automation__SenhaWeb = "senha-web-aqui"
```

4. Execute one worker cycle and observe the orchestration logs.

```powershell
dotnet run --project EmissorNotaFiscal.csproj
```

5. Validate the intended runtime outcome:
   - The orchestrator loads both JSON files.
   - The first billing record is selected deterministically.
   - The runtime dictionary contains the expected recipe keys.
   - The automation service receives the real contract instead of an empty placeholder.
   - Any runtime failure now points to real configuration, credentials, selectors, or portal behavior.

## Expected Outcome

- The old empty-contract failure path is removed from normal orchestration.
- The runtime reaches a real automation attempt backed by the existing JSON files.
- Any remaining failure now reflects real JSON-backed execution rather than the placeholder branch.
