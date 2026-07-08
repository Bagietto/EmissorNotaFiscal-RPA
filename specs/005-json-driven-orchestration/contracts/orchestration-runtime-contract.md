# Contract: Orchestration Runtime

## Purpose

Define the expected orchestration behavior when the runtime switches from the placeholder contract to the real JSON-backed automation path.

## Input Loading

- The orchestrator must load:
  - one automation recipe from `receita_paulistana.json`
  - one billing configuration from `config_notas_v2.json`
- File loading should happen through the existing configuration repository boundary.

## Selection Rule

- The orchestrator must select one deterministic billing item for the real test execution path.
- The current increment does not perform implicit batch execution over all configured items.

## Runtime Dictionary Construction

- The orchestrator must build the dictionary expected by the current recipe using:
  - issuer-level values from the billing configuration
  - selected billing-item values
  - host-supplied credential/configuration values where the billing JSON does not carry them

## Automation Handoff

- The orchestrator must invoke `INfeAutomationService` with:
  - the loaded real contract
  - the constructed runtime dictionary
- Placeholder contract construction must no longer be used in the normal path.

## Failure Behavior

- Missing files, missing billing items, missing required runtime values, and downstream automation failures must remain visible to the caller after orchestration logging.

## Non-Goals

- Full batch execution of all configured customers
- Email delivery integration
- Contract-schema redesign for the existing JSON payloads
