# Quickstart: Consulta de Confirmacao e Download do PDF da NFS-e

Verify that the automation no longer treats issuance completion as the end of the flow and instead confirms the generated note through the portal query area before downloading the official PDF.

## Preconditions

- Project root contains the updated feature artifacts for `015-consulta-download-pdf-nota`.
- The current login, captcha handling, and issuance flow still reach successful final emission.
- Test data is valid enough for the portal to register a note for the current taker.
- The configured downloads directory is accessible for creation and file writing.

## Validation Steps

1. Confirm that the implementation extends the existing issuance flow instead of creating a separate historical-query routine.
2. Run the worker until final emission succeeds for a valid note.
3. Observe that the automation navigates to the query area and filters by the taker document plus the current month and year.
4. Confirm that the automation handles the query results context, including popup behavior when present.
5. Verify that the automation opens the note corresponding to the current execution and triggers the official PDF download.
6. Confirm that the worker ends with a valid filesystem path to the downloaded PDF.
7. Repeat with an invalid download destination and verify that the execution fails before login with a descriptive configuration error.
8. Repeat with a scenario in which the popup does not open or the note is not found and verify that the failure is explicit instead of silently succeeding.

## Expected Results

- A successful issuance run continues into post-emission confirmation instead of stopping immediately after emission.
- The query uses the current execution context to isolate the correct note.
- The automation captures one real PDF file for the same note that was just emitted.
- Invalid download destinations fail before login begins.
- Runs without a usable query result or without a valid PDF do not report success.
