# Data Model: Consulta de Confirmacao e Download do PDF da NFS-e

## Nota Emitida na Execucao

- **Represents**: A nota fiscal gerada pela rodada atual da automacao e que precisa ser confirmada na consulta antes do encerramento.
- **Key Attributes**:
  - `TomadorDocumento`: CPF/CNPJ usado para filtrar a consulta.
  - `PeriodoEmissao`: mes e ano correntes usados como restricao da busca.
  - `IdentificadorDaNota`: referencia mais forte disponivel para diferenciar a nota atual de outros resultados do mesmo tomador.
- **Validation Rules**:
  - Deve existir contexto suficiente para filtrar a consulta do portal pela emissao corrente.
  - Nao pode ser confundida com notas historicas do mesmo cliente no mesmo periodo.

## Consulta de Notas Emitidas

- **Represents**: O conjunto de resultados retornado pelo portal apos o filtro por tomador e periodo.
- **Key Attributes**:
  - `ContextoDeJanela`: local em que os resultados sao exibidos, inclusive popup quando aplicavel.
  - `Resultados`: lista de notas retornadas pela consulta.
  - `Disponibilidade`: indica se houve resultado utilizavel para continuar o fluxo.
- **Validation Rules**:
  - Deve ser acessivel pela automacao sem navegacao manual externa.
  - Deve permitir localizar uma nota correspondente a execucao corrente.

## Resultado de Nota Consultada

- **Represents**: Um item individual da consulta que pode abrir a visualizacao de uma NFS-e especifica.
- **Key Attributes**:
  - `NumeroOuReferenciaExibida`: texto usado para comparar com o identificador da nota atual.
  - `DestinoDeAbertura`: referencia que leva a visualizacao da NFS-e.
- **Validation Rules**:
  - Deve corresponder unicamente a nota emitida na execucao atual antes de disparar o download.

## PDF Oficial da NFS-e

- **Represents**: O artefato final baixado do portal e devolvido ao chamador.
- **Key Attributes**:
  - `CaminhoFisico`: local final do arquivo salvo.
  - `ExistenciaFisica`: garantia de que o arquivo realmente foi gravado.
  - `OrigemDaCaptura`: contexto do navegador em que o download foi produzido.
- **Validation Rules**:
  - O sucesso do servico depende da existencia fisica do arquivo.
  - Arquivos nao PDF ou inexistentes nao satisfazem o contrato final.

## Destino de Download Configurado

- **Represents**: O diretorio configurado para armazenamento do PDF.
- **Key Attributes**:
  - `CaminhoResolvido`: caminho absoluto efetivo usado na execucao.
  - `PermissaoDeEscrita`: capacidade real de criar arquivos no destino.
- **Validation Rules**:
  - Deve existir ou ser criavel antes do bootstrap do navegador.
  - Deve ser validado antes do login; falhas aqui encerram a execucao imediatamente.
