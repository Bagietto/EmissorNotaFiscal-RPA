# Quickstart: Ajuste de Encoding e Seletor do Menu de Emissao

Verify that the automation leaves the authenticated home through a structural menu selector instead of relying on the visible `Emissao de NFS-e` label.

## Preconditions

- Project root contains the updated `receita_paulistana.json`
- The current login flow still reaches the authenticated portal home
- The authenticated page exposes the menu item associated with `nota.aspx`

## Validation Steps

1. Confirm that `receita_paulistana.json` defines `Acessar Menu Lateral Emissao` with a structural selector based on `ctl00_wpMenuLateral_mnuRotinasn3` and `nota.aspx`.
2. Run the worker through the existing login flow until the authenticated home is loaded.
3. Observe that the automation clicks the lateral emission menu without depending on the accented visible label.
4. Confirm that the flow proceeds toward the emission page before the tomador-filling steps run.

## Expected Results

- The menu click no longer times out because of text/encoding mismatch.
- The flow navigates out of the authenticated home into the emission context.
- The login and redirect behavior implemented in earlier features remains unchanged.
