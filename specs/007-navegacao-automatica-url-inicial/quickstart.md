# Quickstart: Navegacao Automatica pela UrlInicial

## Goal

Verify that the engine opens the contract's initial URL automatically before executing any recipe stage.

## Prerequisites

- `.NET 8 SDK` available locally
- Project contains a valid `receita_paulistana.json` with `UrlInicial`
- The portal target is reachable from the machine running the worker

## Validation Flow

1. Build the project.

```powershell
dotnet build EmissorNotaFiscal.csproj --no-restore
```

2. Run the worker with a contract that contains `UrlInicial`.

```powershell
dotnet run --project EmissorNotaFiscal.csproj
```

3. Confirm from the logs and browser state that the first page opened matches the contract URL before any stage steps run.

4. Confirm that later `Navegar` actions still work when the recipe needs an additional transition.

## Expected Outcome

- The contract initial URL is opened automatically at the start of the run.
- The recipe no longer needs a manual bootstrap navigation step.
- Later navigation actions continue to work.
- Missing or invalid `UrlInicial` produces a clear configuration failure.

