# Quickstart: Finalizacao da Emissao com Confirmacao e Download do PDF

Verify that the automation no longer stops after filling the financial fields and instead drives the portal through the final emission step until a PDF download is produced.

## Preconditions

- Project root contains the updated `receita_paulistana.json`
- The current login and menu-navigation flow still reaches the issuance form
- Test data is valid enough for the portal to allow final emission

## Validation Steps

1. Confirm that `receita_paulistana.json` includes a final emission step after the financial fields.
2. Run the worker through the existing flow until description and value are filled.
3. Observe that the automation clicks the final emission control and allows any native confirmation to be handled.
4. Confirm that the portal advances beyond the form and that a real PDF download is captured.
5. Verify that the worker ends with a valid PDF path instead of the previous missing-download error.

## Expected Results

- The automation no longer stops immediately after stage 4.
- The final emission control is clicked.
- Valid issuance runs produce a captured PDF download.
- The service returns a real filesystem path for the emitted PDF.
