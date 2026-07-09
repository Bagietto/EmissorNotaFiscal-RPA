# Feature Specification: Consulta de Confirmacao e Download do PDF da NFS-e

**Feature Branch**: `[015-consulta-download-pdf-nota]`  
**Created**: 2026-07-09  
**Status**: Draft  
**Input**: User description: "Implementar consulta pos-emissao e download do PDF oficial da NFS-e via tela de consultas, popup de resultados e botao btDownload"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Confirmar nota emitida (Priority: P1)

Como operador da automacao, quero que a rotina confirme a nota recem-emitida na area de consultas do portal, para garantir que a emissao foi realmente gravada antes do encerramento do fluxo.

**Why this priority**: Sem essa confirmacao, o processo pode encerrar sem evidenciar se a nota foi persistida pelo portal.

**Independent Test**: Pode ser testado emitindo uma nota valida e verificando que a automacao encontra um resultado correspondente ao tomador e ao periodo corrente na consulta de notas emitidas.

**Acceptance Scenarios**:

1. **Given** que a emissao final da NFS-e foi concluida no portal, **When** a automacao acessar a consulta de notas emitidas, **Then** ela localiza a nota correspondente ao tomador informado na execucao atual.
2. **Given** que a consulta exige filtro por periodo, **When** a automacao executar a busca da nota emitida, **Then** ela utiliza o mes e o ano correntes da emissao para restringir os resultados.

---

### User Story 2 - Obter o PDF oficial da nota (Priority: P2)

Como consumidor do servico de automacao, quero receber o PDF oficial da NFS-e encontrada na consulta, para que o processo termine entregando o documento final correto.

**Why this priority**: A confirmacao da emissao so gera valor completo quando a automacao tambem recupera o documento oficial que sera usado a jusante.

**Independent Test**: Pode ser testado com uma emissao valida e comprovado quando a execucao termina retornando um caminho fisico de PDF existente e correspondente a nota encontrada na consulta.

**Acceptance Scenarios**:

1. **Given** que a nota foi localizada na consulta, **When** a automacao abrir a visualizacao dessa nota, **Then** ela dispara o download do PDF oficial antes de concluir a execucao.
2. **Given** que o documento oficial foi baixado, **When** a automacao encerrar com sucesso, **Then** ela retorna um caminho valido para o arquivo PDF gerado.

---

### User Story 3 - Falhar com contexto acionavel (Priority: P3)

Como operador da automacao, quero receber falhas descritivas quando a nota nao puder ser localizada ou o PDF nao puder ser obtido, para diagnosticar rapidamente se o problema ocorreu na confirmacao, na abertura da nota ou no download final.

**Why this priority**: O fluxo passa a depender de etapas adicionais apos a emissao, o que exige visibilidade clara do ponto de falha para manutencao operacional.

**Independent Test**: Pode ser testado simulando indisponibilidade do resultado da consulta ou ausencia do documento e verificando que a execucao falha sem reportar falso sucesso.

**Acceptance Scenarios**:

1. **Given** que a consulta nao retorna a nota esperada, **When** a automacao concluir a tentativa de confirmacao, **Then** ela encerra com erro explicito em vez de assumir sucesso.
2. **Given** que a nota foi aberta mas o PDF oficial nao puder ser obtido, **When** a execucao atingir a etapa final, **Then** ela falha com indicacao de que o documento final nao foi produzido.

---

### Edge Cases

- O portal pode apresentar mais de uma nota do mesmo tomador no mesmo mes; a automacao deve selecionar a nota correspondente a execucao atual, nao um documento anterior.
- A nota pode levar alguns instantes para aparecer na consulta logo apos a emissao; o fluxo deve tolerar esse atraso dentro da janela operacional esperada.
- A consulta pode abrir em uma janela secundaria; se essa janela nao estiver disponivel, a automacao deve falhar de forma acionavel.
- O download pode ser disparado, mas o arquivo final pode nao estar disponivel ou nao existir ao fim da execucao.
- O diretorio configurado para armazenar PDFs pode estar indisponivel ou sem permissao de escrita antes mesmo do inicio da automacao.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST tratar a consulta pos-emissao como parte obrigatoria do encerramento da rotina de emissao da NFS-e.
- **FR-002**: O sistema MUST acessar a area de consulta de notas emitidas imediatamente apos a conclusao da emissao da nota atual.
- **FR-003**: O sistema MUST filtrar a consulta usando o CPF/CNPJ do tomador informado na execucao atual.
- **FR-004**: O sistema MUST restringir a consulta ao mes e ao ano correntes da emissao.
- **FR-005**: O sistema MUST abrir o resultado da consulta apresentado pelo portal e continuar o fluxo na janela ou contexto em que os resultados forem exibidos.
- **FR-006**: O sistema MUST identificar a nota correspondente a execucao atual entre os resultados retornados pela consulta.
- **FR-007**: O sistema MUST abrir a visualizacao da nota identificada e obter o PDF oficial disponibilizado pelo portal.
- **FR-008**: O sistema MUST concluir com sucesso apenas quando houver um arquivo PDF valido disponivel para retorno ao chamador.
- **FR-009**: O sistema MUST falhar explicitamente quando a nota nao puder ser localizada, aberta ou convertida em um PDF baixado com sucesso.
- **FR-010**: O sistema MUST validar, antes de iniciar a automacao, que o destino configurado para salvar o PDF existe ou pode ser preparado com permissao de escrita.
- **FR-011**: O sistema MUST abortar antes do login quando o destino configurado para o PDF nao puder ser usado, informando um erro descritivo ao operador.
- **FR-012**: O sistema MUST manter o escopo restrito a confirmacao e download da nota emitida na execucao corrente, sem incluir consultas historicas em lote.

### Key Entities *(include if feature involves data)*

- **Nota Emitida na Execucao**: Documento fiscal gerado na rotina atual e usado como alvo unico da confirmacao e do download.
- **Consulta de Notas Emitidas**: Resultado filtrado do portal que relaciona as notas emitidas para o tomador e periodo informados.
- **PDF Oficial da NFS-e**: Arquivo final baixado do portal e usado como criterio de sucesso da automacao.
- **Destino de Download Configurado**: Local informado para armazenamento do PDF e que precisa estar disponivel antes do inicio da execucao.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em execucoes validas nas quais a nota esteja disponivel na consulta do portal, 100% das execucoes bem-sucedidas encerram com um arquivo PDF fisicamente existente.
- **SC-002**: Em execucoes validas, a automacao consegue confirmar a presenca da nota emitida sem navegacao manual adicional fora do fluxo automatizado.
- **SC-003**: Quando o destino configurado para download estiver invalido ou sem permissao, a execucao falha antes do login em 100% dos casos afetados.
- **SC-004**: O sistema nunca reporta sucesso quando a nota nao for localizada na consulta ou quando nenhum PDF final estiver disponivel ao termino do fluxo.

## Assumptions

- A feature cobre apenas a nota emitida na mesma execucao que realizou a emissao, nao incluindo recuperacao historica ampla.
- O operador ja fornece os dados do tomador necessarios para localizar a nota na consulta.
- O portal continua oferecendo consulta por tomador e periodo corrente para notas emitidas.
- O documento baixado pela consulta representa o mesmo PDF oficial esperado no encerramento do processo.
