# Contract: Post-Emission Confirmation and PDF Retrieval

Define the expected behavior for the post-emission segment of the existing NFS-e automation contract.

## Purpose

After the issuance flow completes, the automation must confirm that the note exists in the portal query area and must end only after obtaining the official PDF for that same note.

## Inputs

- One active issuance execution context already authenticated in the portal.
- One runtime data set containing, at minimum:
  - issuer identity already used by the flow
  - taker CPF/CNPJ for the current note
  - current issuance month and year
  - note identifier when available for deterministic selection
- One configured downloads destination that is writable before browser execution proceeds.

## Behavioral Guarantees

- The post-emission steps are part of the same contract execution; they are not a separate historical query routine.
- The automation filters the query using the current taker document and the current issuance period.
- If the portal exposes results in a popup or secondary page, the automation continues in that context without losing the execution state.
- The automation must identify the queried note that belongs to the current execution before triggering PDF retrieval.
- The execution is successful only when one physical PDF file has been captured and validated at the end of the run.

## Failure Guarantees

- If the downloads destination cannot be prepared or written to, the execution fails before login.
- If the queried note cannot be located with the current execution context, the execution fails explicitly.
- If the note view opens but no valid PDF is captured, the execution fails explicitly rather than reporting success.

## Out of Scope

- Bulk historical retrieval of notes across arbitrary periods.
- Post-processing or parsing of the downloaded PDF contents.
- Broader redesign of login, captcha, or unrelated issuance steps.
