# Feature Specification: Suporte a Login Intermediario Opcional

**Feature Branch**: `[006-login-intermediario-opcional]`  
**Created**: 2026-07-03  
**Status**: Draft  
**Input**: User description: "feature para um fluxo de login intermediario, use o arquivo FEATURE-login-intermediario-opcional.md que contem detalhes"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Entrar com ou sem tela intermediaria (Priority: P1)

Como operador da automacao, quero que o fluxo de login continue quando a tela intermediaria aparecer ou nao aparecer, para que a execucao nao falhe por variacao do portal.

**Why this priority**: Esse e o bloqueio principal observado no fluxo real; sem ele, a automacao para antes de acessar a tela de credenciais.

**Independent Test**: Pode ser testado com duas execucoes independentes, uma em que a tela intermediaria aparece e outra em que ela nao aparece, verificando que o fluxo continua ate a etapa de login tradicional.

**Acceptance Scenarios**:

1. **Given** que o portal exibe a tela intermediaria de login, **When** a automacao inicia o fluxo, **Then** ela interage com essa tela e segue para a tela de credenciais sem falhar.
2. **Given** que o portal abre diretamente a tela de credenciais, **When** a automacao inicia o fluxo, **Then** ela ignora a etapa intermediaria e continua normalmente.

---

### User Story 2 - Registrar o caminho seguido (Priority: P2)

Como mantenedor da automacao, quero ver em log qual caminho foi seguido no login, para diagnosticar rapidamente execucoes bem-sucedidas e falhas.

**Why this priority**: O fluxo e variavel, entao a rastreabilidade do caminho executado e essencial para suporte e manutencao.

**Independent Test**: Pode ser validado observando os logs de uma execucao com tela intermediaria e de outra sem a tela, confirmando que o caminho escolhido fica claro.

**Acceptance Scenarios**:

1. **Given** uma execucao com tela intermediaria presente, **When** o login termina a primeira parte do fluxo, **Then** o log indica que o caminho intermediario foi usado.
2. **Given** uma execucao sem tela intermediaria, **When** o login termina a primeira parte do fluxo, **Then** o log indica que o caminho intermediario foi ignorado.

---

### User Story 3 - Preservar o fluxo principal apos a decisao (Priority: P3)

Como usuario da automacao, quero que qualquer erro real depois da decisao sobre a tela intermediaria continue sendo reportado normalmente, para que falhas de portal nao sejam mascaradas.

**Why this priority**: A feature deve resolver apenas a variacao de login; demais falhas precisam continuar visiveis para diagnostico.

**Independent Test**: Pode ser testado ao simular uma falha apos a etapa intermediaria e verificar que a execucao continua reportando o erro real com contexto suficiente.

**Acceptance Scenarios**:

1. **Given** que a decisao sobre a tela intermediaria foi tomada corretamente, **When** ocorre uma falha posterior no login, **Then** a automacao expõe a falha sem tratar como se o fluxo opcional fosse o problema.

---

### Edge Cases

- O portal pode exibir a tela intermediaria por poucos segundos antes de redirecionar automaticamente.
- O portal pode nao exibir a tela intermediaria em nenhuma execucao da mesma rotina.
- O botao ou link da tela intermediaria pode nao estar visivel no momento da tentativa, sem que isso represente erro.
- A tela intermediaria pode mudar de texto ou estrutura visual sem alterar a intencao do fluxo.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST permitir que o fluxo de login continue quando a tela intermediaria estiver presente.
- **FR-002**: O sistema MUST permitir que o fluxo de login continue quando a tela intermediaria nao estiver presente.
- **FR-003**: O sistema MUST representar na receita de automacao qual etapa e opcional no fluxo de login.
- **FR-004**: O sistema MUST registrar se a etapa intermediaria foi executada ou ignorada em cada execucao.
- **FR-005**: O sistema MUST preservar o comportamento normal das etapas subsequentes do login apos a decisao sobre a tela intermediaria.
- **FR-006**: O sistema MUST continuar expondo falhas reais ocorridas depois da etapa intermediaria, sem esconder a causa original.
- **FR-007**: O sistema MUST manter a receita de automacao legivel e previsivel para revisao operacional.

### Key Entities *(include if feature involves data)*

- **Tela Intermediaria de Login**: Etapa opcional apresentada antes da tela principal de credenciais; pode ser usada ou ignorada conforme a presenca no portal.
- **Caminho de Login**: Sequencia de acoes seguida pela automacao entre a abertura da pagina e a chegada ao login tradicional.
- **Registro de Execucao**: Evidencia do caminho seguido e do resultado da decisao tomada durante o login.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em 100% das execucoes de teste, o fluxo nao falha apenas porque a tela intermediaria aparece ou deixa de aparecer.
- **SC-002**: Pelo menos 95% das execucoes de teste registram claramente se a etapa intermediaria foi usada ou ignorada.
- **SC-003**: Em execucoes validas, o fluxo chega ao login tradicional em menos de 30 segundos apos a abertura da pagina inicial.
- **SC-004**: Em pelo menos 9 de 10 execucoes de validacao, a diferenca entre os caminhos seguidos fica clara apenas pelos registros de log.

## Assumptions

- O portal pode apresentar uma variacao intermediaria antes da tela principal de credenciais, mas o objetivo desta feature e tratar apenas essa variacao.
- A receita atual pode ser ajustada para expressar uma etapa opcional sem reorganizar todo o fluxo de automacao.
- O log operacional atual e suficiente para registrar a escolha do caminho sem exigir novo painel ou armazenamento adicional.
- O restante do processo de automacao, depois do login, permanece fora do escopo desta feature.
