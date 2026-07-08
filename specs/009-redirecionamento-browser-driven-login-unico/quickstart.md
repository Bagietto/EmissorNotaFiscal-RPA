# Quickstart: Redirecionamento Browser-Driven do Login Unico

## Goal

Verify that clicking Login Unico waits for the real browser redirect and logs the final URL before credential steps run.

## Validation Flow

1. Build the project.

```powershell
dotnet build EmissorNotaFiscal.csproj --no-restore
```

2. Run the worker against the portal and trigger the Login Unico path.

3. Confirm that the browser navigates through the real redirect and that the final URL is visible in logs.

4. Confirm that the credential selectors are evaluated only after the redirect completes.

## Expected Outcome

- The engine waits for the navigation initiated by Login Unico.
- The final URL after redirect is logged.
- The flow does not rely on a static provider URL.
- The credential steps execute only after the browser reaches the final page.

