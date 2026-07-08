# Feature Specification: Navegacao Automatica pela UrlInicial

**Feature Branch**: `[007-navegacao-automatica-url-inicial]`  
**Created**: 2026-07-03  
**Status**: Draft  
**Input**: User description: "FEATURE-navegacao-automatica-url-inicial.md"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Abrir a pagina inicial automaticamente (Priority: P1)

Como operador da automacao, quero que o motor abra automaticamente a pagina inicial declarada no contrato, para que a receita nao precise inserir um passo de bootstrap manual.

**Why this priority**: Essa e a correcao principal do comportamento atual e reduz diretamente a fragilidade do fluxo.

**Independent Test**: Pode ser testado executando um contrato com `UrlInicial` configurada e verificando que a primeira pagina aberta corresponde a essa URL antes de qualquer passo da receita.

**Acceptance Scenarios**:

1. **Given** um contrato com `UrlInicial` valida, **When** a execucao comeca, **Then** o motor navega automaticamente para essa URL antes de processar as etapas.
2. **Given** uma receita sem passo manual de navegacao, **When** a execucao comeca, **Then** o fluxo ainda inicia na pagina correta informada pelo contrato.

---

### User Story 2 - Manter logs e falhas coerentes com o bootstrap (Priority: P2)

Como mantenedor da automacao, quero ver logs claros sobre a navegacao inicial e falhas explicitas quando a URL inicial estiver ausente ou invalida, para diagnosticar rapidamente problemas de configuracao.

**Why this priority**: A navegacao automatica precisa ser rastreavel e deve falhar de forma util quando o contrato nao fornecer uma URL inicial aproveitavel.

**Independent Test**: Pode ser testado com um contrato valido e com um contrato sem `UrlInicial`, confirmando que o primeiro gera log de bootstrap e o segundo falha de forma clara.

**Acceptance Scenarios**:

1. **Given** um contrato com `UrlInicial` valida, **When** o bootstrap de navegacao ocorre, **Then** o log registra a URL usada.
2. **Given** um contrato sem `UrlInicial` util, **When** a execucao inicia, **Then** o motor falha com mensagem clara antes de executar as etapas.

---

### User Story 3 - Preservar navegacoes adicionais na receita (Priority: P3)

Como usuario da automacao, quero que as acoes `Navegar` da receita continuem funcionando normalmente, para que o fluxo possa mudar de pagina quando isso for necessario no meio do processo.

**Why this priority**: A navegacao automatica nao deve eliminar a flexibilidade atual do fluxo.

**Independent Test**: Pode ser testado com um contrato que tenha uma URL inicial e uma acao `Navegar` posterior, verificando que ambas as navegacoes acontecem na ordem esperada.

**Acceptance Scenarios**:

1. **Given** uma URL inicial e uma acao `Navegar` em uma etapa posterior, **When** a execucao roda, **Then** o bootstrap acontece uma vez e a navegacao adicional continua funcionando.

---

### Edge Cases

- A URL inicial pode estar vazia, nula ou formada de maneira invalida.
- A receita pode ainda conter um passo `Navegar` adicional depois do bootstrap automatico.
- O contrato pode ser reutilizado em fluxos futuros onde a primeira pagina venha de outro mecanismo, mas isso nao e o foco desta entrega.
- A navegacao inicial pode concluir com a pagina ainda carregando quando os proximos passos forem iniciados.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST navegar automaticamente para `contrato.UrlInicial` antes de iniciar a iteracao das etapas.
- **FR-002**: O sistema MUST executar a navegacao inicial apenas uma vez por execucao do contrato.
- **FR-003**: O sistema MUST registrar em log a URL usada no bootstrap de navegacao inicial.
- **FR-004**: O sistema MUST falhar de forma clara quando a `UrlInicial` estiver ausente ou invalida em um contrato que depende desse bootstrap.
- **FR-005**: O sistema MUST manter o comportamento das acoes `Navegar` declaradas na receita para mudancas de pagina posteriores.
- **FR-006**: O sistema MUST preservar o modelo assincrono atual do motor de automacao.

### Key Entities *(include if feature involves data)*

- **Contrato de Automacao**: Estrutura raiz que define a URL inicial e as etapas de execucao.
- **Bootstrap de Navegacao**: Abertura inicial da pagina antes do processamento da receita.
- **Acao de Navegacao**: Passo da receita que continua disponivel para transicoes adicionais ao longo do fluxo.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em 100% das execucoes com `UrlInicial` valida, a primeira pagina aberta corresponde a URL declarada no contrato.
- **SC-002**: Em pelo menos 95% das execucoes de validacao, os logs identificam claramente a URL usada no bootstrap.
- **SC-003**: Quando a `UrlInicial` estiver ausente ou invalida, a execucao falha antes de processar etapas e expõe uma mensagem clara de configuracao.
- **SC-004**: Em execucoes com navegacao adicional, a etapa `Navegar` posterior continua operando sem regressao funcional.

## Assumptions

- O contrato real ja expõe `UrlInicial` como a pagina de entrada principal do fluxo.
- A receita pode continuar usando `Navegar` para transicoes intermediarias ou posteriores.
- O comportamento de bootstrap deve ser implementado no motor, sem exigir alteracoes no orquestrador.
- A ausencia de `UrlInicial` e tratada como erro de configuracao para este fluxo atual.
