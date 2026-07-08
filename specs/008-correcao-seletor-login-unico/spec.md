# Feature Specification: Correcao do Seletor da Tela de Login Unico

**Feature Branch**: `[008-correcao-seletor-login-unico]`  
**Created**: 2026-07-03  
**Status**: Draft  
**Input**: User description: "FEATURE-correcao-seletor-login-unico.md"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Clicar corretamente no Login Unico (Priority: P1)

Como operador da automacao, quero que a receita identifique o botao real de Login Unico, para que a etapa opcional nao seja ignorada quando a tela intermediaria estiver presente.

**Why this priority**: Essa correção resolve a causa imediata da automacao permanecer presa na tela intermediaria.

**Independent Test**: Pode ser testada executando a automacao em uma pagina que exiba o botao de Login Unico e confirmando que o clique acontece com o seletor atualizado.

**Acceptance Scenarios**:

1. **Given** que a tela intermediaria de Login Unico esta presente, **When** a receita executa a etapa opcional, **Then** o botao correto e acionado.
2. **Given** que o botao de Login Unico nao esta presente, **When** a etapa opcional roda, **Then** o fluxo continua sem quebra.

---

### User Story 2 - Avancar para a tela de credenciais (Priority: P2)

Como usuario da automacao, quero que o fluxo saia da tela intermediaria depois do clique correto, para que os campos `#cpfCnpj` e `#password` aparecam e possam ser preenchidos.

**Why this priority**: O objetivo da correção e restabelecer o caminho até a tela principal de autenticação.

**Independent Test**: Pode ser testada observando que, após o clique no Login Unico, a tela de credenciais se torna disponível.

**Acceptance Scenarios**:

1. **Given** que o Login Unico foi acionado, **When** o portal responde normalmente, **Then** a automacao segue para a tela de credenciais.

---

### Edge Cases

- O portal pode alterar o texto visivel do botao sem mudar sua estrutura HTML.
- O texto visivel pode aparecer com acentacao diferente entre ambientes ou respostas do servidor.
- O botao pode continuar existindo mesmo quando o comportamento visual da pagina muda.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST usar um seletor aderente ao HTML real do botao de Login Unico.
- **FR-002**: O sistema MUST acionar a etapa opcional de Login Unico quando a tela intermediaria estiver presente.
- **FR-003**: O sistema MUST continuar ignorando a etapa opcional quando a tela intermediaria nao estiver presente.
- **FR-004**: O sistema MUST permitir que o fluxo avance para a tela de credenciais apos o clique correto.
- **FR-005**: O sistema MUST evitar dependencia de texto fragil quando houver um seletor estrutural mais confiavel.

### Key Entities *(include if feature involves data)*

- **Botao de Login Unico**: Elemento da tela intermediaria que inicia a transicao para o login principal.
- **Tela Intermediaria**: Pagina transitória exibida antes da autenticacao principal.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em execucoes com tela intermediaria presente, o Login Unico e acionado em 100% dos casos de validacao.
- **SC-002**: Em execucoes validas, a automacao chega aos campos de credenciais sem permanecer presa na tela intermediaria.
- **SC-003**: Em execucoes sem a tela intermediaria, o fluxo continua sem regredir o comportamento atual.

## Assumptions

- O seletor estrutural do botao e mais estavel que o texto exibido.
- O restante do fluxo de login permanece inalterado.
- A correção pode ser feita apenas na receita JSON.
