# Data Model: Ajuste de Encoding e Seletor do Menu de Emissao

## Menu Navigation Target

- **Represents**: O alvo configurado para o passo `Acessar Menu Lateral Emissao` dentro da receita autenticada.
- **Source**: `receita_paulistana.json`
- **Relevant Attributes**:
  - `Descricao`: representa o clique que sai da home autenticada para a tela de emissao.
  - `PlaywrightAcao`: permanece `ClicarBotao`.
  - `SeletorHtml`: passa a expressar um criterio estrutural baseado em id e destino funcional.

## Authenticated Portal Menu Item

- **Represents**: O item do menu lateral exposto ao usuario apos login bem-sucedido.
- **Why it matters**: E o primeiro alvo acionavel apos a autenticacao e o ponto de transicao para a emissao.

## Encoding-Sensitive Visible Label

- **Represents**: O texto renderizado do menu, hoje suscetivel a diferencas de acento e encoding.
- **Why it matters**: Explica por que a estrategia anterior baseada em texto deixou de ser confiavel.
