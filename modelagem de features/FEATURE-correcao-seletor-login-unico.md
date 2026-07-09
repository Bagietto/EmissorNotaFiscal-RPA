# Feature: Correcao do Seletor da Tela de Login Unico

## Contexto

Depois da implementacao de:

- suporte a etapa opcional de login intermediario
- navegacao automatica pela `UrlInicial`

o fluxo de automacao continuou falhando antes de chegar aos campos `#cpfCnpj` e `#password`.

Foi realizada uma investigacao sobre o HTML real servido em `https://nfe.prefeitura.sp.gov.br/login.aspx` para entender por que a etapa opcional estava sendo ignorada, mesmo quando a pagina inicial exibe o botao de entrada via Login Unico.

## Problema Real Encontrado

O problema principal nao parece mais estar no motor de automacao.

A causa mais provavel identificada foi um desencontro entre o seletor configurado na receita e o texto real presente na pagina:

- seletor atual na receita: `text=Login unico`
- texto real no HTML servido pelo portal: `Login único`

Ou seja, a receita procura uma string sem acento, enquanto a pagina real apresenta o texto com acento.

## Evidencia da Investigacao

No HTML atual da pagina de login, o botao e renderizado com a seguinte estrutura:

- `button.oauth-button`
- `span.oauth-name`
- conteudo visivel: `Login único`

Isso explica o comportamento observado:

1. a navegacao inicial agora acontece corretamente
2. a etapa opcional nao encontra `text=Login unico`
3. o clique do Login Unico nao acontece
4. a automacao permanece na tela intermediaria
5. o passo seguinte tenta localizar `#cpfCnpj`
6. esse campo nunca aparece, porque o fluxo ainda nao saiu da tela intermediaria

## Conclusao

O gargalo atual e de receita/seletor, nao de bootstrap de navegacao e, provavelmente, nem da logica opcional em si.

Em outras palavras:

- a feature de navegacao automatica resolveu um problema real
- a feature de passo opcional tambem resolveu outro problema estrutural
- o fluxo ainda falha porque o seletor configurado para o Login Unico nao corresponde ao texto real da interface

## Objetivo da Feature

Corrigir a receita de automacao para usar um seletor robusto e aderente ao HTML real da tela intermediaria de Login Unico.

## Resultado Esperado

A automacao deve:

1. localizar corretamente o botao de Login Unico quando ele estiver presente
2. clicar nesse botao
3. prosseguir para a tela onde os campos `#cpfCnpj` e `#password` passam a existir
4. ignorar a etapa somente quando a tela intermediaria realmente nao estiver presente

## Recomendacao de Ajuste

Trocar o seletor atual baseado em texto sem acento por um seletor mais fiel ao HTML real.

Opcoes mais promissoras:

- `text=Login único`
- `button.oauth-button`
- `span.oauth-name`

## Recomendacao Inicial

Dar preferencia a um seletor estrutural do botao, como `button.oauth-button`, porque ele tende a ser menos sensivel a variacoes textuais, acentuacao ou mudancas cosmeticas no conteudo visivel.

## Escopo Funcional

Esta feature deve cobrir:

- correcao do seletor do passo opcional de Login Unico em `receita_paulistana.json`
- validacao de que o clique acontece quando a tela intermediaria estiver presente
- confirmacao de que o fluxo avanca para a tela de credenciais

## Fora de Escopo

Esta feature nao cobre:

- mudancas adicionais no motor de automacao
- revisao completa dos seletores das etapas posteriores
- alteracoes no orquestrador

## Criterios de Aceite

- A etapa opcional nao deve ser ignorada quando a pagina exibir o botao de Login Unico.
- O clique no Login Unico deve acontecer com o seletor atualizado.
- A automacao deve deixar de falhar em `#cpfCnpj` por permanecer indevidamente na tela intermediaria.
- Se a tela intermediaria nao aparecer, o passo opcional deve continuar sendo ignorado sem quebrar o fluxo.

## Arquivos Provavelmente Impactados em Implementacao Futura

- `receita_paulistana.json`
- opcionalmente documentacao complementar em `specs/`

## Perguntas em Aberto

- `button.oauth-button` sera estavel o suficiente entre releases do portal?
- Apos o clique no Login Unico, sera necessario incluir uma espera adicional de navegacao antes de buscar `#cpfCnpj`?
