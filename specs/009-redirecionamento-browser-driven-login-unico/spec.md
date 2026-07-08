# Feature Specification: Redirecionamento Browser-Driven do Login Unico

**Feature Branch**: `[009-redirecionamento-browser-driven-login-unico]`  
**Created**: 2026-07-03  
**Status**: Draft  
**Input**: User description: "FEATURE-redirecionamento-browser-driven-login-unico.md"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Seguir o redirect real do navegador (Priority: P1)

Como operador da automacao, quero que o clique em Login Unico aguarde a navegacao real do navegador, para que o fluxo siga o redirecionamento efetivo do portal em vez de tentar abrir uma URL estatica.

**Why this priority**: Esse e o comportamento raiz do fluxo de autenticacao; sem ele, o login pode ficar preso em uma transicao incompleta.

**Independent Test**: Pode ser testado executando o fluxo com Login Unico e confirmando que o navegador avanca para o dominio de autenticacao esperado apos o clique.

**Acceptance Scenarios**:

1. **Given** que o Login Unico esta disponivel, **When** a automacao clica no botao, **Then** ela aguarda a navegacao real do navegador antes de prosseguir.
2. **Given** que o redirecionamento acontece, **When** a navegacao termina, **Then** o fluxo continua a partir da pagina final carregada.

---

### User Story 2 - Registrar a URL final apos o redirecionamento (Priority: P2)

Como mantenedor da automacao, quero ver em log a URL final atingida depois do clique, para diagnosticar rapidamente o destino real do redirect.

**Why this priority**: A investigacao mostrou que o destino do login e dinamico, entao a visibilidade da URL final e importante para suporte.

**Independent Test**: Pode ser validado observando os logs de uma execucao em que o clique em Login Unico ocorre e conferindo a URL registrada ao final da navegacao.

**Acceptance Scenarios**:

1. **Given** uma navegacao concluida apos o Login Unico, **When** o fluxo registra o resultado, **Then** a URL final aparece no log.

---

### User Story 3 - Preservar os seletores da tela final (Priority: P3)

Como usuario da automacao, quero que a receita continue usando os seletores corretos da tela de autenticacao final, para que o fluxo chegue aos campos de credenciais depois do redirect.

**Why this priority**: O redirect precisa terminar em uma tela utilizavel; depois dele, os seletores devem continuar apontando para os campos reais.

**Independent Test**: Pode ser testado validando que, depois da navegacao, os campos de credenciais podem ser localizados e preenchidos.

**Acceptance Scenarios**:

1. **Given** que o redirecionamento concluiu, **When** a automacao procura os campos finais, **Then** ela encontra a tela de autenticacao correta.

---

### Edge Cases

- O redirecionamento pode demorar mais do que o normal em ambientes lentos.
- A URL final pode conter parametros dinamicos variaveis a cada execucao.
- O fluxo pode responder com bloqueio ou pagina transitória se o navegador nao respeitar o redirect completo.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST aguardar a navegacao real do navegador apos o clique em Login Unico.
- **FR-002**: O sistema MUST continuar a partir da pagina final carregada apos o redirect.
- **FR-003**: O sistema MUST registrar em log a URL final atingida apos o redirecionamento.
- **FR-004**: O sistema MUST evitar depender de uma URL estatica hardcoded para o provedor de autenticacao.
- **FR-005**: O sistema MUST manter os seletores da tela final de autenticacao disponiveis para os passos seguintes.

### Key Entities *(include if feature involves data)*

- **Redirect de Login Unico**: Transicao iniciada pelo navegador apos o clique no botao intermediario.
- **URL Final de Autenticacao**: Destino real alcançado depois do redirecionamento.
- **Tela Final de Credenciais**: Pagina carregada apos o redirect onde os campos de acesso devem ser encontrados.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em execucoes validas com Login Unico, a automacao segue o redirect real em 100% dos casos de validacao.
- **SC-002**: A URL final apos o redirecionamento aparece nos logs em pelo menos 95% das execucoes observadas.
- **SC-003**: A automacao nao depende de abrir manualmente uma URL estatica do provedor de autenticacao.
- **SC-004**: Em execucoes validas, os campos de credenciais continuam acessiveis apos a navegacao final.

## Assumptions

- O clique no Login Unico e o gatilho oficial do redirect.
- O fluxo continua partindo da URL inicial da NFS-e.
- O ajuste pode exigir apenas o motor de automacao e, se necessario, pequenos refinamentos na receita.
