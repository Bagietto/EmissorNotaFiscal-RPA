# Feature: Ajuste de Encoding e Seletor do Menu de Emissao

## Contexto

Depois que o fluxo de Login Unico passou a autenticar corretamente, a automacao deixou de falhar na etapa de login e passou a quebrar no primeiro passo apos a autenticacao.

O erro observado foi timeout ao tentar localizar o item de menu responsavel por abrir a tela de emissao da NFS-e.

## Problema Real Encontrado

O problema principal nao esta mais no login.

A investigacao mostrou que a automacao chegou na area autenticada do portal, mas o seletor atual do menu:

- `text=Emissao de NFS-e`

e fragil para a estrutura real da pagina autenticada.

O HTML real retornado pelo portal utiliza:

- `charset=iso-8859-1`
- item de menu com link estrutural para `nota.aspx`
- elemento identificado dentro do menu lateral por `ctl00_wpMenuLateral_mnuRotinasn3`

Ou seja, o fluxo depende hoje de um texto visivel sujeito a acentuacao, encoding e variacoes de renderizacao, quando existe um alvo estrutural mais confiavel no DOM.

## Evidencia da Investigacao

No HTML autenticado capturado apos o submit do login foi encontrado:

- `meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1"`
- `id="ctl00_wpMenuLateral_mnuRotinasn3"`
- `href="nota.aspx"`
- texto visivel `Emissao de NFS-e`

Tambem foi confirmado visualmente que, apos o login, o portal ja estava na home autenticada e o menu lateral estava disponivel.

Isso explica o comportamento observado:

1. o login ocorre com sucesso
2. a navegacao chega na area autenticada
3. o motor tenta localizar o menu por texto
4. o locator baseado em texto nao encontra o alvo esperado dentro do prazo
5. a automacao falha antes de entrar na tela de emissao

## Conclusao

O gargalo atual e de seletor e resiliencia contra encoding, nao de autenticacao.

Em outras palavras:

- o fluxo de Login Unico ja consegue levar o navegador ate a area restrita
- a quebra atual acontece no primeiro clique do menu lateral
- a feature correta agora e substituir a dependencia de texto por um seletor estrutural aderente ao HTML real

## Objetivo da Feature

Ajustar a etapa de acesso ao menu de emissao para usar um seletor estrutural, menos sensivel a encoding, acentos e pequenas mudancas cosmeticas da interface.

## Resultado Esperado

A automacao deve:

1. reconhecer de forma confiavel o item de menu que leva para a emissao de NFS-e
2. deixar de depender exclusivamente do texto visivel acentuado
3. clicar no menu lateral correto apos o login
4. prosseguir para a tela de emissao antes de iniciar o preenchimento do tomador

## Recomendacao de Ajuste

Trocar o seletor atual baseado em texto por um seletor estrutural do link de emissao.

Opcoes mais promissoras:

- `a[href="nota.aspx"]`
- seletor que combine o container do menu lateral com `href="nota.aspx"`
- opcionalmente um fallback que valide tambem o texto visivel

## Recomendacao Inicial

Dar preferencia a um seletor estrutural baseado em `href="nota.aspx"`, porque ele representa o destino funcional real da navegacao e tende a ser menos fragil do que um locator textual.

## Escopo Funcional

Esta feature deve cobrir:

- ajuste do seletor do passo `Acessar Menu Lateral Emissao` em `receita_paulistana.json`
- revisao da estrategia para evitar dependencia de encoding textual
- validacao de que o clique leva o fluxo para a pagina de emissao

## Fora de Escopo

Esta feature nao cobre:

- alteracoes no login ou no redirecionamento do Login Unico
- tratamento de captcha, challenge ou `mcaptcha`
- revisao completa dos demais seletores de formulario
- mudancas no orquestrador

## Criterios de Aceite

- A automacao deve localizar o menu de emissao mesmo quando o portal responder em `iso-8859-1`.
- O passo `Acessar Menu Lateral Emissao` nao deve depender exclusivamente de texto acentuado.
- O clique deve abrir a tela de emissao da NFS-e com consistencia.
- O fluxo deve conseguir sair da home autenticada e avancar para a etapa de preenchimento do tomador.

## Arquivos Provavelmente Impactados em Implementacao Futura

- `receita_paulistana.json`
- opcionalmente `specs/`

## Perguntas em Aberto

- O melhor alvo sera apenas `a[href="nota.aspx"]` ou vale combinar com o container do menu lateral?
- Vale manter um fallback textual apenas para diagnostico?
- Existem outros menus com o mesmo destino funcional em perfis diferentes do portal?
