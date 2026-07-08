# Quickstart: Suporte a Login Intermediario Opcional

## Goal

Verify that the login flow continues correctly whether the intermediate `Login unico` screen appears or not.

## Prerequisites

- `.NET 8 SDK` available locally
- Project root contains `receita_paulistana.json`
- The worker can read the current JSON recipe through the existing repository abstraction
- The portal under test is reachable from the machine running the worker

## Validation Flow

1. Build the project.

```powershell
dotnet build EmissorNotaFiscal.csproj --no-restore
```

2. Run the worker in an environment where the intermediate login screen appears.

```powershell
dotnet run --project EmissorNotaFiscal.csproj
```

3. Confirm from the logs that the intermediate path was used and the flow reached the standard credential screen.

4. Run the worker again in an environment where the intermediate screen does not appear.

5. Confirm from the logs that the intermediate path was skipped and the flow still reached the standard credential screen.

## Expected Outcome

- The login flow completes when the optional screen is present.
- The login flow completes when the optional screen is absent.
- Logs clearly identify which path was taken in each run.
- Failures after the decision point remain visible and attributable to the real downstream step.

