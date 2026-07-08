# Feature Specification: Ajuste do Seletor do Botao Entrar e Consolidacao das Investigacoes de Login

**Feature Branch**: `[010-ajuste-botao-entrar-login]`  
**Created**: 2026-07-04  
**Status**: Draft  
**Input**: User description: "FEATURE-ajuste-seletor-botao-entrar-e-investigacoes-login.md"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Clicar no botao final correto (Priority: P1)

Como operador da automacao, quero que o clique final de login aponte apenas para o botao tradicional de `Entrar`, para que o fluxo envie as credenciais sem colidir com a alternativa `Entrar com gov.br`.

**Why this priority**: O fluxo agora alcanca a tela real de autenticacao; sem esse ajuste, a automacao falha no ultimo clique por ambiguidade de seletor.

**Independent Test**: Pode ser testado executando o fluxo ate a tela de autenticacao, preenchendo `#cpfCnpj` e `#password`, e confirmando que o clique final ocorre sem erro de modo estrito.

**Acceptance Scenarios**:

1. **Given** que a tela real de autenticacao esta carregada, **When** a automacao executa o passo final de login, **Then** ela clica apenas no botao tradicional de `Entrar`.
2. **Given** que existe tambem a opcao `Entrar com gov.br`, **When** a automacao executa o clique final, **Then** o fluxo nao falha por ambiguidade de seletor.

---

### User Story 2 - Preservar o fluxo ja investigado (Priority: P2)

Como mantenedor da automacao, quero consolidar nesta feature o estado atual das investigacoes de login, para que o proximo ajuste parta de um historico claro do que ja foi validado.

**Why this priority**: O problema atual e incremental; registrar o que ja foi destravado evita retrabalho e regressao de diagnostico.

**Independent Test**: Pode ser validado revisando a documentacao da feature e confirmando que ela descreve os marcos ja comprovados do fluxo de autenticacao.

**Acceptance Scenarios**:

1. **Given** que houve varias investigacoes anteriores no login, **When** a feature e consultada, **Then** ela resume os ajustes ja implementados e o bloqueio atual.

---

### User Story 3 - Manter a navegacao ate a tela de credenciais (Priority: P3)

Como usuario da automacao, quero que os campos `#cpfCnpj` e `#password` continuem acessiveis antes do clique final, para que a correcao do seletor nao altere os passos ja estabilizados.

**Why this priority**: O botao final so faz sentido se o restante do fluxo continuar chegando corretamente a tela do `pmspauth`.

**Independent Test**: Pode ser testado confirmando que a automacao continua encontrando os campos de credenciais antes do envio do formulario.

**Acceptance Scenarios**:

1. **Given** que o Login Unico redirecionou corretamente, **When** a automacao chega na tela final, **Then** os campos `#cpfCnpj` e `#password` continuam sendo preenchidos antes do clique.

---

### Edge Cases

- O botao `Entrar com gov.br` pode compartilhar classes visuais com o botao tradicional.
- O formulario tradicional pode continuar presente mesmo quando existem opcoes alternativas de autenticacao.
- O seletor final precisa permanecer compativel com a tela real sem reintroduzir dependencias textuais frageis.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST usar um seletor final de login que identifique somente o botao tradicional de envio das credenciais.
- **FR-002**: O sistema MUST evitar colisao com a opcao `Entrar com gov.br` ao executar o clique final.
- **FR-003**: O sistema MUST preservar o preenchimento bem-sucedido de `#cpfCnpj` e `#password` antes do envio do formulario.
- **FR-004**: O sistema MUST manter o fluxo atual que chega a tela real de autenticacao apos o `Login unico`.
- **FR-005**: O sistema MUST registrar nesta feature o historico consolidado das investigacoes relevantes para o fluxo de login.

### Key Entities *(include if feature involves data)*

- **Botao Final de Entrar**: Elemento da tela real de autenticacao que envia o formulario tradicional de usuario e senha.
- **Opcao Entrar com gov.br**: Acao alternativa de autenticacao exibida na mesma tela, mas que nao deve ser acionada neste fluxo.
- **Estado Consolidado da Investigacao**: Registro dos marcos ja validados no fluxo de login antes deste ajuste incremental.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em 100% das validacoes executadas na tela real de autenticacao, o clique final nao falha por ambiguidade de seletor.
- **SC-002**: Em 100% das validacoes executadas, a automacao continua preenchendo `#cpfCnpj` e `#password` antes do envio do formulario.
- **SC-003**: O fluxo nao aciona a opcao `Entrar com gov.br` nas execucoes do login tradicional observadas.
- **SC-004**: A documentacao da feature registra explicitamente os sete marcos investigativos confirmados antes deste ajuste.

## Assumptions

- O botao correto de envio do formulario tradicional continua sendo um `submit` distinto da opcao `gov.br`.
- O fluxo browser-driven do `Login unico` ja foi estabilizado e nao faz parte do escopo deste ajuste.
- O ajuste principal pode ser resolvido na receita de automacao, sem nova mudanca obrigatoria no motor.
