# Feature Specification: Modo Assistido para Caption no PMSP Auth

**Feature Branch**: `[012-modo-assistido-caption-pmspauth]`  
**Created**: 2026-07-04  
**Status**: Draft  
**Input**: User description: "FEATURE-modo-assistido-caption-pmspauth.md"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Pausar o fluxo para resolucao humana do challenge (Priority: P1)

Como operador da automacao, quero que o navegador permaneça aberto e o fluxo pause quando o `pmspauth` exibir um challenge/caption, para que eu possa resolver manualmente a validacao sem perder a sessao automatizada.

**Why this priority**: Sem essa pausa controlada, o fluxo falha exatamente quando o provedor exige validacao humana.

**Independent Test**: Pode ser testado executando o login com modo assistido habilitado e confirmando que o worker entra em espera ao detectar a tela de challenge.

**Acceptance Scenarios**:

1. **Given** que o modo assistido esta habilitado, **When** o `pmspauth` exibe uma tela de challenge/caption, **Then** o navegador permanece aberto e a automacao entra em espera controlada.
2. **Given** que o challenge esta presente, **When** a automacao entra em espera, **Then** o log informa claramente que a resolucao humana e aguardada.

---

### User Story 2 - Retomar automaticamente apos a intervencao humana (Priority: P2)

Como operador da automacao, quero que o worker retome sozinho quando o portal sair do challenge e voltar a um estado funcional valido, para nao precisar reiniciar manualmente o restante do fluxo.

**Why this priority**: O valor do modo assistido esta em preservar o restante da automacao depois da intervencao humana.

**Independent Test**: Pode ser validado resolvendo manualmente o challenge e observando que o fluxo avanca sozinho quando a continuidade real fica disponivel.

**Acceptance Scenarios**:

1. **Given** que o operador resolveu o challenge corretamente, **When** o portal volta a um estado valido do fluxo, **Then** a automacao retoma automaticamente.
2. **Given** que a pagina apenas mudou sem liberar a continuidade real, **When** o worker reavalia o estado, **Then** ele nao retoma prematuramente.

---

### User Story 3 - Encerrar com timeout claro quando nao houver resposta humana (Priority: P3)

Como mantenedor da automacao, quero um timeout configuravel para a espera assistida, para que o worker encerre com diagnostico claro quando nao houver intervencao do operador.

**Why this priority**: Um modo assistido sem limite de espera dificulta a operacao e a observabilidade do worker.

**Independent Test**: Pode ser testado deixando o challenge sem resolucao ate o limite configurado e verificando a mensagem final de timeout.

**Acceptance Scenarios**:

1. **Given** que o challenge permaneceu sem resolucao dentro do prazo, **When** o limite de espera for atingido, **Then** o fluxo encerra com mensagem clara de timeout do modo assistido.

---

### Edge Cases

- O challenge pode reaparecer mais de uma vez na mesma sessao.
- O provedor pode manter o operador em `pmspauth` mesmo apos uma tentativa humana invalida.
- A pagina pode sair do challenge sem ainda expor os elementos reais de continuidade do fluxo.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST permitir habilitar um modo assistido para a automacao do `pmspauth`.
- **FR-002**: O sistema MUST executar o navegador em modo visivel quando o modo assistido estiver habilitado.
- **FR-003**: O sistema MUST detectar a presenca da tela de challenge/caption por elementos estruturais confiaveis.
- **FR-004**: O sistema MUST pausar a progressao automatica enquanto o challenge estiver presente e registrar em log que aguarda intervencao humana.
- **FR-005**: O sistema MUST retomar o fluxo apenas quando houver um indicio funcional claro de continuidade valida.
- **FR-006**: O sistema MUST encerrar com timeout configuravel e mensagem clara quando a resolucao humana nao ocorrer no prazo.

### Key Entities *(include if feature involves data)*

- **Modo Assistido**: Configuracao operacional que mantem o navegador visivel e permite intervencao humana durante o challenge.
- **Tela de Challenge**: Estado do `pmspauth` em que o provedor exige validacao humana antes de liberar a continuidade do login.
- **Estado Valido de Continuidade**: Condicao em que o challenge desapareceu e o fluxo volta a expor sinais reais de progresso automatizado.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em 100% das execucoes validadas com challenge presente e modo assistido habilitado, o worker nao falha imediatamente ao detectar a tela.
- **SC-002**: Em 100% das execucoes validadas em que o operador resolve corretamente o challenge dentro do prazo, o fluxo retoma automaticamente.
- **SC-003**: Em 100% das execucoes validadas sem resolucao humana dentro do prazo, o worker encerra com mensagem clara de timeout assistido.
- **SC-004**: O log registra explicitamente a entrada e a saida do estado de espera assistida.

## Assumptions

- O `pmspauth` continuara expondo marcadores estruturais relacionados a `mcaptcha` quando o challenge estiver presente.
- O proximo indicio funcional valido apos a resolucao humana sera a disponibilidade de elementos das etapas seguintes ou a saida do dominio `pmspauth`.
- A primeira entrega deve privilegiar operabilidade assistida em vez de qualquer tentativa de resolucao automatica do challenge.
