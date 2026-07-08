# Quickstart: Ajuste do Seletor do Botao Entrar e Consolidacao das Investigacoes de Login

Verify that the final login click targets only the traditional submit button after the Login Unico redirect has already reached the real authentication page.

## Preconditions

- Project root contains the updated `receita_paulistana.json`
- The current flow still reaches the `pmspauth` credential screen
- Runtime data provides valid values for `CnpjPrestador` and `SenhaWeb`

## Validation Steps

1. Confirm that `receita_paulistana.json` defines `Disparar Clique de Login` with `button.btn-entrar[type="submit"]`.
2. Run the worker against the portal and let the flow follow the existing `Login unico` path when present.
3. Observe that the automation reaches the credential screen, fills `#cpfCnpj` and `#password`, and executes the final click without strict-mode ambiguity.
4. Review logs to ensure the earlier redirect behavior remains intact and no new selector collision is reported.

## Expected Results

- The final login step resolves to exactly one visible element.
- The automation does not click `Entrar com gov.br`.
- The existing intermediary-login and browser-driven redirect behavior remains unchanged.
