# Data Model: Modo Assistido para Caption no PMSP Auth

## Assisted Automation Options

- **Represents**: As configuracoes operacionais do modo assistido.
- **Source**: `appsettings.json` and command-line overrides
- **Relevant Attributes**:
  - `Enabled`: define se a execucao assistida deve ficar ativa.
  - `HumanInterventionTimeoutMinutes`: define o prazo maximo para a resolucao manual.

## Challenge State

- **Represents**: O estado do navegador quando o `pmspauth` exige validacao humana.
- **Why it matters**: Esse estado interrompe a progressao automatica e exige espera controlada.

## Continuation Signal

- **Represents**: O conjunto de sinais que comprovam que o fluxo voltou a um caminho valido apos a intervencao humana.
- **Why it matters**: Impede que o worker retome apenas porque a pagina mudou superficialmente.
