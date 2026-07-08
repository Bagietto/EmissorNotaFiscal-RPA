# Data Model: Ajuste do Seletor do Botao Entrar e Consolidacao das Investigacoes de Login

## Recipe Action Target

- **Represents**: O alvo configurado para o passo `Disparar Clique de Login` dentro da receita de autenticacao.
- **Source**: `receita_paulistana.json`
- **Relevant Attributes**:
  - `Descricao`: identifica semanticamente o clique final de login.
  - `PlaywrightAcao`: permanece `ClicarBotao`.
  - `SeletorHtml`: passa a distinguir o submit tradicional da opcao alternativa de autenticacao.

## Login Page Action Choice

- **Represents**: A diferenciacao entre o botao tradicional de envio das credenciais e a acao alternativa `Entrar com gov.br`.
- **Why it matters**: O motor opera em modo estrito; portanto, o seletor precisa resolver para um unico elemento visivel.

## Investigation State Snapshot

- **Represents**: O conjunto de descobertas ja confirmadas no fluxo de login antes deste ajuste incremental.
- **Why it matters**: Permite validar que a nova feature atua apenas sobre o ultimo bloqueio conhecido.
