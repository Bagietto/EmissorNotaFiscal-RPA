# Quickstart: Contract-Driven Playwright Automation

## Goal

Verify that the automation service boundary and Playwright engine implementation match the feature scope for generic contract execution, runtime data injection, and PDF path return behavior.

## Prerequisites

- `.NET 8 SDK` available locally
- Active feature branch `003-playwright-contract-engine`
- Playwright package already restored through the project
- Local Chromium runtime available through Playwright installation flow when execution testing is needed

## Implementation Outline

1. Add `INfeAutomationService` under `Domain/Interfaces/`.
2. Add `ContractBasedAutomationEngine` under `Infrastructure/Automation/`.
3. Reuse the existing automation contract models under `Domain/Models/Automation/`.
4. Keep execution fully asynchronous and metadata-driven.
5. Return one physical PDF path only when a download has actually been captured.

## Suggested Validation Flow

1. Restore and build the project.

```powershell
dotnet restore EmissorNotaFiscal.csproj --ignore-failed-sources
dotnet build EmissorNotaFiscal.csproj --no-restore
```

2. Inspect that the new source files exist only in the expected folders.

```powershell
Get-ChildItem Domain\Interfaces -Recurse
Get-ChildItem Infrastructure\Automation -Recurse
```

3. Validate the engine behavior in follow-up implementation or tests:
   - Ordered stage execution from contract metadata
   - Runtime dictionary value injection for text input steps
   - Automatic dialog acceptance when requested
   - Immediate failure with stage/action context on step errors
   - Concrete PDF path return only when a download is captured

## Expected Outcome

- The repository contains a clean automation boundary in `Domain/Interfaces`.
- Browser-specific execution is isolated in `Infrastructure/Automation`.
- The engine remains generic and reusable across different portal contracts without hardcoded selectors.
