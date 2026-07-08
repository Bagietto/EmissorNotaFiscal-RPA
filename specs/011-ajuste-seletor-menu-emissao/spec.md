# Feature Specification: Ajuste de Encoding e Seletor do Menu de Emissao

**Feature Branch**: `[011-ajuste-seletor-menu-emissao]`  
**Created**: 2026-07-04  
**Status**: Draft  
**Input**: User description: "FEATURE-ajuste-encoding-seletor-menu-emissao.md"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Acessar o menu lateral de emissao com confiabilidade (Priority: P1)

Como operador da automacao, quero que o menu lateral de emissao seja localizado por um alvo estrutural do DOM, para que o fluxo saia da home autenticada e entre na tela de emissao da NFS-e sem depender de texto acentuado.

**Why this priority**: Esse e o primeiro bloqueio apos o login bem-sucedido; sem ele, a automacao nao avanca para a emissao.

**Independent Test**: Pode ser testado executando o fluxo ate a area autenticada e confirmando que o clique do menu lateral abre a tela de emissao sem timeout por locator textual.

**Acceptance Scenarios**:

1. **Given** que o usuario ja esta autenticado no portal, **When** a automacao executa o passo `Acessar Menu Lateral Emissao`, **Then** ela localiza o item de menu por um seletor estrutural.
2. **Given** que o portal renderiza o texto com acento ou encoding variavel, **When** a automacao tenta abrir a emissao, **Then** o fluxo nao depende exclusivamente do texto visivel.

---

### User Story 2 - Resistir a variacoes de encoding do portal (Priority: P2)

Como mantenedor da automacao, quero que a receita continue funcional mesmo quando o portal responder em `iso-8859-1`, para reduzir fragilidade de encoding na navegacao autenticada.

**Why this priority**: A investigacao mostrou que o problema atual e seletor sensivel a encoding, nao falha de autenticacao.

**Independent Test**: Pode ser validado observando que o clique do menu continua funcional em uma resposta autenticada que exponha o mesmo destino estrutural mesmo com texto renderizado de forma diferente.

**Acceptance Scenarios**:

1. **Given** que a pagina autenticada usa um charset diferente do esperado, **When** o menu lateral e exibido, **Then** a automacao ainda encontra o link de emissao pelo seu destino funcional.

---

### User Story 3 - Preservar a transicao para o preenchimento do tomador (Priority: P3)

Como usuario da automacao, quero que o fluxo chegue na tela de emissao antes de iniciar o preenchimento do tomador, para que as etapas seguintes continuem coerentes com o contrato atual.

**Why this priority**: O clique no menu e apenas o elo de passagem para as etapas ja existentes do formulario.

**Independent Test**: Pode ser testado verificando que, apos o clique no menu, o fluxo consegue prosseguir para os campos da etapa de tomador.

**Acceptance Scenarios**:

1. **Given** que o clique no menu lateral ocorreu com sucesso, **When** a automacao prossegue, **Then** ela sai da home autenticada e entra no contexto de emissao antes das etapas de tomador.

---

### Edge Cases

- O item do menu pode manter o mesmo destino funcional e mudar apenas o texto visivel.
- O identificador estrutural pode estar no proprio link ou em seu container imediato.
- Pode existir mais de um item visual semelhante, mas apenas um com destino `nota.aspx`.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST localizar o menu lateral de emissao por um seletor estrutural aderente ao HTML autenticado.
- **FR-002**: O sistema MUST evitar dependencia exclusiva do texto visivel `Emissao de NFS-e`.
- **FR-003**: O sistema MUST manter o fluxo funcional quando o portal responder com encoding que altere a renderizacao textual.
- **FR-004**: O sistema MUST acionar o item de menu que leva ao destino funcional de emissao da NFS-e.
- **FR-005**: O sistema MUST preservar a sequencia atual em que a automacao autentica, abre o menu de emissao e so entao continua para o preenchimento do tomador.

### Key Entities *(include if feature involves data)*

- **Menu de Emissao**: Item da navegacao lateral disponivel na area autenticada que leva a tela de emissao da NFS-e.
- **Destino Funcional de Emissao**: Alvo estrutural do menu associado a navegacao para `nota.aspx`.
- **Home Autenticada**: Pagina do portal apos login bem-sucedido, onde o menu lateral passa a ficar disponivel.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em 100% das validacoes executadas na home autenticada, o passo `Acessar Menu Lateral Emissao` deixa de falhar por timeout do locator textual anterior.
- **SC-002**: Em 100% das validacoes executadas, o fluxo nao depende exclusivamente do texto acentuado do menu para localizar a navegacao de emissao.
- **SC-003**: O clique do menu leva o navegador para a tela de emissao com consistencia nas execucoes observadas.
- **SC-004**: O fluxo consegue sair da home autenticada e avancar para a fase de preenchimento apos o clique estrutural do menu.

## Assumptions

- O destino funcional `nota.aspx` continua sendo o caminho correto para a tela de emissao da NFS-e.
- O identificador `ctl00_wpMenuLateral_mnuRotinasn3` continua associado ao item de menu correto no perfil atualmente suportado.
- O ajuste pode ser resolvido na receita sem necessidade de nova logica obrigatoria no motor de automacao.
