# Quickstart: Correcao do Seletor da Tela de Login Unico

## Goal

Verify that the optional Login Unico step clicks the real button and allows the flow to reach the credential screen.

## Validation Flow

1. Update the recipe selector to the structural button selector.
2. Build the project.

```powershell
dotnet build EmissorNotaFiscal.csproj --no-restore
```

3. Run the worker against the portal and confirm the intermediate step no longer gets stuck on the login screen.

## Expected Outcome

- The Login Unico button is matched by `button.oauth-button`.
- The flow advances to `#cpfCnpj` and `#password` when the intermediate screen is present.
- The optional step remains safely ignored when the screen is absent.
