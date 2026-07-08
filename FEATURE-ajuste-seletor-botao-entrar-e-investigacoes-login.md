# Feature: Ajuste do Seletor do Botao Entrar e Consolidacao das Investigacoes de Login

## Objetivo

Registrar o ajuste necessario para continuar a automacao a partir da tela real de login e senha, junto com o historico das implementacoes e descobertas feitas durante a investigacao do fluxo de autenticacao da NFS-e Paulistana.

## Estado Atual Confirmado

Depois das ultimas investigacoes e ajustes, a automacao conseguiu:

1. abrir a pagina inicial da NFS-e
2. acionar corretamente o `Login unico`
3. seguir o redirecionamento ate a tela real de autenticacao do dominio `pmspauth.prefeitura.sp.gov.br`
4. encontrar os campos `#cpfCnpj` e `#password`

O proximo bloqueio passou a ser o clique do botao final de login.

## Problema Atual

O seletor atual da receita para o envio do login e:

- `button.btn-entrar`

Na tela real de autenticacao, esse seletor resolve para dois elementos:

1. o botao `Entrar`
2. o botao `Entrar com gov.br`

Como o Playwright esta em modo estrito, a automacao falha com ambiguidade de seletor em vez de clicar no botao correto.

## Ajuste Necessario

Refinar o seletor do passo `Disparar Clique de Login` para apontar especificamente para o botao de submit do formulario tradicional de usuario e senha.

## Resultado Esperado

Com esse ajuste, a automacao deve:

1. preencher `#cpfCnpj`
2. preencher `#password`
3. clicar no botao correto de `Entrar`
4. evitar o botao alternativo `Entrar com gov.br`

## Recomendacao Inicial de Ajuste

Substituir o seletor generico por um seletor mais especifico, priorizando o botao submit do formulario principal.

Exemplos de direcao:

- botao com `type=\"submit\"`
- seletor por formulario e botao submit
- seletor textual exato para `Entrar`, desde que nao colida com o fluxo `gov.br`

## Implementacoes Ja Realizadas Nesta Investigacao

### 1. Suporte a fluxo opcional de login intermediario

Foi implementado suporte para que a automacao consiga continuar quando a tela intermediaria de `Login unico` aparece ou nao aparece.

Impactos realizados:

- extensao do motor para lidar com passo opcional
- suporte ao tipo de acao `ClicarSeExistir`
- adaptacao da receita para representar o login intermediario como etapa opcional

### 2. Navegacao automatica pela `UrlInicial`

Foi implementado bootstrap automatico da execucao usando `contrato.UrlInicial`, evitando depender de um passo manual de navegacao na receita.

Impactos realizados:

- navegacao automatica no inicio da execucao do contrato
- validacao explicita da `UrlInicial`
- log de bootstrap da navegacao inicial

### 3. Correcao do seletor do `Login unico`

Foi identificado que o seletor antigo nao correspondia corretamente ao HTML real da pagina.

Impactos realizados:

- troca do seletor do passo opcional para `button.oauth-button`

### 4. Investigacao do redirecionamento real do Login Unico

Foi confirmado que o fluxo correto nao deve usar uma URL fixa hardcoded do `pmspauth` como ponto de entrada.

Descobertas confirmadas:

- o clique no `Login unico` submete `POST` para `/connect/login`
- esse endpoint devolve `302`
- o navegador e redirecionado para `connect/authorize` com parametros dinamicos como `state` e `code_challenge`
- o fluxo deve continuar browser-driven

### 5. Espera explicita de navegacao apos o `Login unico`

Foi ajustado o motor para aguardar a navegacao disparada pelo clique no `Login unico`, em vez de tentar continuar imediatamente para os proximos passos.

Impactos realizados:

- `WaitForURLAsync` apos o clique
- log da URL final atingida

### 6. Diagnostico de falhas de navegacao

Foi adicionada instrumentacao temporaria/diagnostica para entender por que o navegador caia em `chrome-error://chromewebdata/`.

Impactos realizados:

- log de `RequestFailed`
- log de `PageError`
- captura de screenshot e HTML ao cair em `chrome-error://chromewebdata/`

### 7. Confirmacao de diferenca entre headless e headed

Foi confirmado experimentalmente que:

- em `headless=true`, o redirect para `pmspauth` falhou com `ERR_CONNECTION_RESET`
- em `headless=false`, o fluxo conseguiu chegar a tela real de autenticacao

Essa descoberta foi essencial para destravar a investigacao e confirmar que os campos de login estavam corretos.

## Conclusao Atual

O fluxo de autenticacao avancou de forma significativa. O problema nao esta mais:

- na URL inicial
- no clique do `Login unico`
- no redirecionamento browser-driven
- nos campos `#cpfCnpj` e `#password`

O bloqueio atual esta concentrado no seletor do botao final de envio das credenciais.

## Proximo Passo Recomendado

Ajustar o seletor do passo:

- `Disparar Clique de Login`

para que ele aponte somente para o botao correto de `Entrar`, sem colidir com `Entrar com gov.br`.

## Arquivos Provavelmente Impactados

- `receita_paulistana.json`
- possivelmente `Infrastructure/Automation/ContractBasedAutomationEngine.cs` se for necessario suportar um criterio de clique mais especifico

## Criterios de Aceite

- A automacao deve continuar chegando ate a tela real de login do `pmspauth`.
- Os campos `#cpfCnpj` e `#password` devem continuar sendo preenchidos com sucesso.
- O clique final deve ocorrer no botao correto de `Entrar`.
- O fluxo nao deve mais falhar por ambiguidade do seletor `button.btn-entrar`.
