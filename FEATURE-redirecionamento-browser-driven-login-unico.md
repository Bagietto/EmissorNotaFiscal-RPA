# Feature: Redirecionamento Browser-Driven do Login Unico

## Contexto

Durante a investigacao do fluxo de autenticacao da NFS-e Paulistana, foi confirmado que o botao `Login unico` nao leva diretamente a uma URL fixa e estavel de login.

O fluxo real observado e:

1. o navegador abre `https://nfe.prefeitura.sp.gov.br/login.aspx`
2. o clique no botao `Login unico` submete um `POST` para `https://nfe.prefeitura.sp.gov.br/connect/login`
3. esse endpoint responde com `302 Found`
4. o `Location` do redirecionamento aponta para `https://pmspauth.prefeitura.sp.gov.br/connect/authorize?...`
5. essa URL contem parametros dinamicos de OAuth, incluindo `state` e `code_challenge`

## Problema Real Encontrado

O problema nao e simplesmente descobrir uma URL fixa para substituir em `UrlInicial`.

O fluxo de autenticacao depende de uma navegacao iniciada pelo navegador, com parametros dinamicos gerados no momento do clique. Quando tentamos acessar o ambiente `pmspauth` diretamente, sem passar pelo fluxo original, encontramos bloqueios.

## Evidencias da Investigacao

### Evidencia 1: o clique gera um POST e nao um link direto

Foi confirmado que o botao `Login unico` dispara um `POST` para:

- `https://nfe.prefeitura.sp.gov.br/connect/login`

Ao enviar um payload equivalente ao formulario (`qs=`), o endpoint respondeu com:

- `HTTP/1.1 302 Found`
- `Location: https://pmspauth.prefeitura.sp.gov.br/connect/authorize?...`

### Evidencia 2: a URL de authorize e dinamica

O redirecionamento retornado contem parametros variaveis por requisicao, como:

- `state`
- `code_challenge`
- `code_challenge_method`

Isso indica um fluxo OAuth/OIDC com dados efemeros, nao uma tela de login fixa que possa ser aberta de forma estatica na receita.

### Evidencia 3: acesso direto ao login do auth falhou no Playwright

Quando a `UrlInicial` foi alterada para:

- `https://pmspauth.prefeitura.sp.gov.br/account/login`

o Playwright falhou ja no bootstrap com:

- `net::ERR_CONNECTION_RESET`

### Evidencia 4: acesso manual ao authorize fora do fluxo retornou bloqueio

Ao tentar abrir diretamente a URL de `connect/authorize` obtida no redirect, a resposta retornou uma pagina com:

- `Requisicao Bloqueada`

Isso sugere protecao do portal, validacao de contexto, WAF, ou dependencia do fluxo completo do navegador.

## Conclusao

O fluxo correto para chegar a tela de login e senha nao deve ser modelado como:

- trocar a `UrlInicial` por uma URL direta do `pmspauth`

O caminho correto deve continuar sendo:

1. abrir `https://nfe.prefeitura.sp.gov.br/login.aspx`
2. clicar no `Login unico`
3. deixar o navegador seguir o redirect real gerado pelo servidor
4. aguardar a navegacao ate a pagina de autenticacao efetiva
5. so entao localizar os campos de login e senha reais dessa tela

## Objetivo da Feature

Ajustar a automacao para respeitar o redirecionamento real iniciado pelo botao `Login unico`, tratando a navegacao como parte do fluxo do navegador, em vez de tentar abrir diretamente uma URL fixa do provedor de autenticacao.

## Resultado Esperado

A automacao deve:

1. iniciar no portal original da NFS-e
2. clicar em `Login unico`
3. aguardar a navegacao/redirect para o dominio `pmspauth.prefeitura.sp.gov.br`
4. detectar a tela de autenticacao real apos o redirecionamento
5. usar os seletores corretos dessa nova tela

## Escopo Funcional

Esta feature deve cobrir:

- preservacao da `UrlInicial` em `https://nfe.prefeitura.sp.gov.br/login.aspx`
- uso do clique no `Login unico` como gatilho oficial do fluxo
- espera explicita por navegacao/redirect apos o clique
- instrumentacao de log que mostre a URL final apos o redirect
- revisao dos seletores da tela de autenticacao final

## Fora de Escopo

Esta feature nao cobre:

- bypass manual do fluxo OAuth
- substituicao do redirect por URL fixa hardcoded
- automacao de fluxos paralelos fora do Login Unico

## Requisitos Funcionais

- A automacao deve iniciar no portal original da NFS-e.
- O clique em `Login unico` deve continuar sendo o disparador do fluxo.
- A aplicacao deve aguardar a navegacao apos esse clique antes de procurar campos de credenciais.
- O fluxo nao deve depender de uma URL estatica hardcoded para `pmspauth`.
- A automacao deve registrar a URL final atingida apos o redirecionamento.

## Requisitos Tecnicos

- O motor deve tratar o clique em `Login unico` como acao que pode resultar em navegacao cross-domain.
- Pode ser necessario incluir uma espera explicita por URL, load state ou seletor da tela final.
- Os seletores `#cpfCnpj` e `#password` devem ser revalidados na tela final real do `pmspauth`.
- A implementacao deve evitar simular chamadas HTTP manuais como substitutas do fluxo do navegador.

## Criterios de Aceite

- A `UrlInicial` permanece apontando para `https://nfe.prefeitura.sp.gov.br/login.aspx`.
- O fluxo nao tenta mais abrir diretamente `https://pmspauth.prefeitura.sp.gov.br/account/login`.
- A automacao consegue seguir o redirect disparado por `Login unico`.
- A URL final apos o clique e observavel em log para depuracao.
- O proximo passo da investigacao ou implementacao passa a focar os seletores reais da tela final de autenticacao.

## Arquivos Provavelmente Impactados em Implementacao Futura

- `receita_paulistana.json`
- `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- possivelmente documentacao em `specs/`

## Proxima Hipotese Tecnica

Depois de respeitar o redirect browser-driven, o proximo ajuste provavelmente sera:

- aguardar a URL ou o carregamento da pagina final de autenticacao
- revisar os seletores reais da tela que surge dentro de `pmspauth`
