# Data Model: Finalizacao da Emissao com Confirmacao e Download do PDF

## Final Emission Step

- **Represents**: A nova etapa do contrato que conclui a emissao da nota apos o preenchimento dos campos financeiros.
- **Source**: `receita_paulistana.json`
- **Relevant Attributes**:
  - `Descricao`: identifica o disparo final da emissao.
  - `PlaywrightAcao`: usa o clique padrao do motor.
  - `SeletorHtml`: expressa um alvo estrutural para o comando final de emitir.

## Emission Confirmation

- **Represents**: A interacao intermediaria possivelmente exigida pelo portal apos o clique de emissao.
- **Why it matters**: Pode ser o ponto que separa o clique final do inicio real da geracao do documento.

## Downloaded PDF

- **Represents**: O artefato final baixado pelo navegador e devolvido pelo servico.
- **Why it matters**: E o criterio de sucesso operacional do fluxo completo.
