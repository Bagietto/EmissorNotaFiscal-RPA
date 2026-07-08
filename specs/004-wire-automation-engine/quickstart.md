# Quickstart: Wire Automation Engine

## Goal

Verify that the automation engine is registered in the host, the orchestrator depends on the abstraction boundary, and the runtime configuration path exposes the minimum automation settings.

## Prerequisites

- `.NET 8 SDK` available locally
- Active feature branch `004-wire-automation-engine`
- Prior automation features already present in the working tree

## Implementation Outline

1. Register `INfeAutomationService` to `ContractBasedAutomationEngine` in `Program.cs`.
2. Extend `appsettings.json` with an `Automation` section containing `Headless` and `DownloadsDirectory`.
3. Update `FaturamentoOrchestrator` to receive the automation abstraction through constructor injection.
4. Keep orchestration behavior minimal and abstraction-driven.

## Suggested Validation Flow

1. Restore and build the project.

```powershell
dotnet restore EmissorNotaFiscal.csproj
dotnet build EmissorNotaFiscal.csproj --no-restore
```

2. Inspect the automation registration and orchestration boundary.

```powershell
Get-Content Program.cs
Get-Content Application\FaturamentoOrchestrator.cs
Get-Content appsettings.json
```

3. Validate the intended runtime outcome:
   - The host can resolve the automation abstraction.
   - The orchestrator depends on `INfeAutomationService`.
   - Automation settings are visible through normal configuration.

## Expected Outcome

- The automation engine participates in the application dependency graph.
- The orchestration boundary is connected to automation through the abstraction.
- Operators can configure automation visibility and download output without code changes.
