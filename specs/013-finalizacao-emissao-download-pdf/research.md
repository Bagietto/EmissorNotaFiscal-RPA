# Research: Finalizacao da Emissao com Confirmacao e Download do PDF

## Decision 1: Completar a receita antes de mudar o engine

- **Decision**: Adicionar uma etapa 5 de finalizacao da emissao em `receita_paulistana.json`.
- **Rationale**: O engine ja registra downloads e aceita dialogs automaticamente. A falha atual decorre do fato de o contrato terminar antes do comando final de emissao.

## Decision 2: Usar seletor estrutural composto para o controle final de emissao

- **Decision**: Mirar o clique final com um seletor composto baseado em `btEmitir` e em controles `input`/`button` relacionados.
- **Rationale**: A feature nao trouxe o HTML final do botao, mas o padrao ASP.NET do portal e consistente com controles estruturais do tipo `btEmitir`. Isso e mais defensavel do que depender apenas de texto visivel.
- **Alternatives considered**:
  - Esperar por investigacao adicional antes de tocar a receita: descartado porque o objetivo desta iteracao e justamente completar a fase final faltante.
  - Reestruturar o engine para detectar download sem clique final: descartado por atacar o sintoma, nao a lacuna do fluxo.

## Decision 3: Reaproveitar o tratamento global de dialogos

- **Decision**: Nao adicionar nova logica de confirmacao ao engine nesta iteracao.
- **Rationale**: O fluxo ja registra `TratarDialogos` cedo na autenticacao, e o engine aceita dialogs nativos automaticamente. O primeiro passo pragmatica e verificar se a confirmacao do portal cai nesse mecanismo.
