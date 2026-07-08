# Research: Ajuste do Seletor do Botao Entrar e Consolidacao das Investigacoes de Login

## Decision 1: Refinar o clique final na receita

- **Decision**: Atualizar o seletor do passo `Disparar Clique de Login` para `button.btn-entrar[type="submit"]`.
- **Rationale**: O problema relatado e ambiguidade em modo estrito porque `button.btn-entrar` resolve tanto o submit tradicional quanto a alternativa `Entrar com gov.br`. Priorizar `type="submit"` atende a recomendacao inicial da feature e reduz risco sem exigir nova logica no engine.
- **Alternatives considered**:
  - Seletor textual exato para `Entrar`: descartado por maior fragilidade a variacoes textuais.
  - Seletor mais profundo por formulario: adiado ate existir evidencia de que `type="submit"` ainda e ambiguo.

## Decision 2: Manter o motor de automacao inalterado

- **Decision**: Nao alterar `Infrastructure/Automation/ContractBasedAutomationEngine.cs` nesta iteracao.
- **Rationale**: O motor ja cobre o fluxo opcional, o redirect browser-driven, os logs de URL final e a instrumentacao diagnostica. O bloqueio atual esta isolado no seletor do passo final.
- **Alternatives considered**:
  - Adicionar heuristica de clique no engine: descartado por aumentar escopo sem evidencia de necessidade.

## Decision 3: Consolidar o historico da investigacao na nova feature

- **Decision**: Registrar no pacote `010` os marcos ja validados do login para manter rastreabilidade do estado atual.
- **Rationale**: A feature recebida ja atua como consolidacao do aprendizado anterior e define claramente o que saiu de escopo e o que continua bloqueado.
