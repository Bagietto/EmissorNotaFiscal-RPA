# Feature Specification: Finalizacao da Emissao com Confirmacao e Download do PDF

**Feature Branch**: `[013-finalizacao-emissao-download-pdf]`  
**Created**: 2026-07-04  
**Status**: Draft  
**Input**: User description: "FEATURE-finalizacao-emissao-e-confirmacao-download-pdf.md"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Disparar a emissao final da nota (Priority: P1)

Como operador da automacao, quero que o fluxo acione o comando real de emissao depois de preencher os campos financeiros, para que a nota avance alem do formulario e entre no processamento final.

**Why this priority**: Sem o clique final de emissao, o fluxo sempre para antes da geracao do documento.

**Independent Test**: Pode ser testado executando o fluxo ate a etapa financeira e confirmando que a automacao deixa de encerrar imediatamente apos preencher descricao e valor.

**Acceptance Scenarios**:

1. **Given** que os campos financeiros ja foram preenchidos, **When** a automacao conclui a etapa 4, **Then** ela aciona o comando final de emissao.
2. **Given** que o portal exige uma interacao de envio final, **When** a automacao executa a etapa de finalizacao, **Then** o fluxo avanca alem do formulario de preenchimento.

---

### User Story 2 - Compatibilizar a confirmacao exigida pelo portal (Priority: P2)

Como operador da automacao, quero que o fluxo atravesse a confirmacao exibida pelo portal quando ela aparecer, para que a emissao nao pare entre o clique final e a geracao do documento.

**Why this priority**: A investigacao ja antecipa que a emissao pode exigir uma confirmacao antes de prosseguir.

**Independent Test**: Pode ser validado observando que, quando houver confirmacao nativa do navegador, a automacao nao fica bloqueada nesse ponto.

**Acceptance Scenarios**:

1. **Given** que o portal exibe uma confirmacao para prosseguir, **When** a automacao chega a esse momento, **Then** ela atravessa a confirmacao e continua o fluxo.

---

### User Story 3 - Encerrar com um PDF baixado de verdade (Priority: P3)

Como usuario da automacao, quero que a execucao termine com um caminho de PDF valido, para que o servico conclua com sucesso e entregue o documento emitido.

**Why this priority**: O contrato do servico so considera a execucao concluida quando um PDF e capturado.

**Independent Test**: Pode ser testado emitindo uma nota valida e confirmando que o worker retorna um caminho real de PDF em vez do erro de ausencia de download.

**Acceptance Scenarios**:

1. **Given** que a emissao final foi processada com sucesso, **When** o portal gerar o documento, **Then** a automacao produz um download de PDF valido.
2. **Given** que o download real foi disparado, **When** o engine encerra a execucao, **Then** ele retorna um caminho de arquivo existente.

---

### Edge Cases

- O comando final de emissao pode variar entre controles ASP.NET `input` e `button`.
- A confirmacao pode ocorrer como dialogo nativo ou nao ocorrer em todos os cenarios.
- O processamento do portal pode levar alguns segundos entre o clique final e o inicio do download.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST incluir uma etapa explicita de finalizacao da emissao apos o preenchimento dos campos financeiros.
- **FR-002**: O sistema MUST acionar o comando real de emissao da nota dentro do fluxo automatizado.
- **FR-003**: O sistema MUST preservar a compatibilidade com confirmacoes nativas do navegador quando elas surgirem apos o clique final.
- **FR-004**: O sistema MUST aguardar a continuidade do processamento final antes de concluir a execucao.
- **FR-005**: O sistema MUST encerrar a execucao com um caminho de PDF valido quando a emissao for concluida corretamente.

### Key Entities *(include if feature involves data)*

- **Comando Final de Emissao**: Controle acionado apos o preenchimento dos dados da nota para iniciar a emissao definitiva.
- **Confirmacao de Emissao**: Interacao intermediaria que pode ser exigida pelo portal antes da geracao do documento.
- **PDF Emitido**: Documento final baixado pelo fluxo e usado como criterio de sucesso do servico.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em 100% das validacoes com dados validos, a automacao deixa de encerrar imediatamente apos preencher descricao e valor.
- **SC-002**: Em 100% das validacoes em que o portal permite a emissao, o fluxo aciona o comando final da nota.
- **SC-003**: Em execucoes validas com documento gerado, o erro `A automacao foi concluida sem produzir um caminho de PDF baixado.` deixa de ocorrer.
- **SC-004**: Em execucoes validas com emissao concluida, o servico retorna um caminho de PDF existente ao final.

## Assumptions

- O gargalo principal atual esta na ausencia da etapa final na receita, nao no mecanismo de captura de download do engine.
- O portal continua aceitando confirmacoes nativas do navegador por meio do tratamento de dialogos ja existente.
- O controle final de emissao expoe ao menos um identificador estrutural relacionado a `btEmitir` ou ao valor funcional `Emitir`.
