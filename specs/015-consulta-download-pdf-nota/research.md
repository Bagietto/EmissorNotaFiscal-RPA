# Research: Consulta de Confirmacao e Download do PDF da NFS-e

## Decision 1: Tratar a consulta pos-emissao como extensao do fluxo atual, nao como nova rotina paralela

- **Decision**: Acrescentar as etapas de confirmacao e download ao contrato existente de emissao, preservando um unico fluxo ponta a ponta.
- **Rationale**: A spec define a consulta como etapa de encerramento da emissao corrente. Separar em uma segunda rotina aumentaria ambiguidade sobre contexto, dados dinamicos e criterio de sucesso.
- **Alternatives considered**:
  - Criar um segundo contrato exclusivo de consulta: descartado por fragmentar o processo e exigir coordenacao adicional entre duas execucoes.
  - Fazer verificacao manual fora do contrato: descartado porque enfraquece o criterio automatizado de conclusao.

## Decision 2: Evoluir o enum de acoes e o engine para suportar popup, dropdown e selecao contextual

- **Decision**: Adicionar tipos de acao especificos para selecao de dropdown, clique com troca para popup, abertura da nota por correspondencia dinamica e disparo explicito do download.
- **Rationale**: A receita atual cobre apenas cliques/preenchimentos simples na mesma pagina. O fluxo descrito na feature introduz mudanca de contexto de janela e busca dirigida na lista de resultados, o que nao cabe de forma segura nos comandos existentes.
- **Alternatives considered**:
  - Codificar toda a consulta diretamente no engine sem novos tipos de acao: descartado por reduzir a expressividade do contrato JSON e acoplar o fluxo ao portal.
  - Tentar reutilizar apenas `ClicarBotao` com seletores mais complexos: descartado porque nao resolve a necessidade de popup nem a selecao dirigida da nota.

## Decision 3: Usar dados da execucao atual para filtrar a consulta e identificar a nota alvo

- **Decision**: Fornecer ao contrato, no minimo, CPF/CNPJ do tomador, mes/ano da emissao e, quando disponivel, um identificador forte da nota emitida para localizar o link correto no popup.
- **Rationale**: O principal risco funcional da feature e baixar um PDF errado quando houver varias notas do mesmo tomador no mesmo periodo. Quanto mais forte o contexto dinamico da execucao, menor a chance de falso positivo.
- **Alternatives considered**:
  - Clicar sempre no primeiro resultado da consulta: descartado por ser frágil e sujeito a baixar documento de outra emissao.
  - Expandir o escopo para busca historica e heuristicas mais amplas: descartado por sair da feature atual.

## Decision 4: Validar o diretorio de downloads de forma fail-fast antes do bootstrap do navegador

- **Decision**: Converter a verificacao do diretorio configurado em etapa antecipada obrigatoria, com tentativa de criacao e teste de escrita antes do login.
- **Rationale**: A spec exige que erros de permissao/caminho abortem a execucao cedo, evitando processamento inutil num fluxo que so tem valor se puder devolver o PDF final.
- **Alternatives considered**:
  - Confiar apenas no `Directory.CreateDirectory` tardio: descartado porque nao cobre teste real de escrita nem produz diagnostico claro antes do login.
  - Tratar a falha apenas no momento do download: descartado porque posterga erro de configuracao para o final do processo.

## Decision 5: Manter a captura de download centralizada no engine

- **Decision**: Reaproveitar o mecanismo central de captura de PDF ja existente, estendendo-o para cobrir o contexto em que o download possa ocorrer apos popup/nova pagina.
- **Rationale**: O contrato do servico ja depende de um caminho fisico de PDF ao final. Reutilizar essa infraestrutura reduz mudancas desnecessarias e concentra a garantia de sucesso/falha num unico ponto.
- **Alternatives considered**:
  - Fazer o contrato retornar apenas um evento de sucesso sem caminho do arquivo: descartado porque quebraria o contrato existente do servico.
  - Mover a responsabilidade do caminho do PDF para o orquestrador: descartado por duplicar responsabilidade fora do engine.
