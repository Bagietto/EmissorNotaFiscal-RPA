# Data Model: Suporte a Login Intermediario Opcional

## Entities

### LoginIntermediarioOpcional

Represents the optional screen that may appear before the standard credential form.

**Attributes**
- `Nome` - human-readable label for the intermediate step
- `SinalizadorOpcional` - indicates that the step can be skipped without failing the flow
- `SeletorDeReconhecimento` - identifier used by the automation recipe to locate the screen action

**Rules**
- Must not block the flow when absent.
- Must be logged when used or skipped.

### CaminhoDeLogin

Represents the path taken during a single automation run from the landing page to the credential form.

**Attributes**
- `TipoDeCaminho` - indicates whether the intermediate screen was used or bypassed
- `EtapasExecutadas` - ordered list of login-related steps followed in the run
- `ResultadoDaDecisao` - outcome of the optional step evaluation

**Rules**
- Must always reach the standard login credentials step when the portal allows it.
- Must preserve the order of subsequent login actions.

### RegistroDeExecucao

Represents the operational evidence needed to support troubleshooting.

**Attributes**
- `Mensagem` - log message describing the branch selected
- `EtapaRelacionada` - optional reference to the step that generated the message
- `Timestamp` - moment the decision or action was recorded

**Rules**
- Must distinguish between the optional path being executed and being skipped.
- Must remain readable in the worker logs.

## Relationship Summary

- A `CaminhoDeLogin` may contain zero or one `LoginIntermediarioOpcional` decision point.
- Each automation run should produce at least one `RegistroDeExecucao` describing the chosen path.

## State Flow

1. The portal opens.
2. The automation checks whether the intermediate login screen is available.
3. If present, the optional path is used and logged.
4. If absent, the flow continues directly and logs that the path was skipped.
5. Subsequent login actions run normally.

