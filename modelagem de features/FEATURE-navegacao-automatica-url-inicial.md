# Feature: Navegacao Automatica pela UrlInicial

## Contexto

Durante a investigacao do fluxo de login da automacao, foi identificado que o contrato `FluxoAutomacaoContrato` ja possui a propriedade `UrlInicial`, mas o motor atual so navega para uma pagina quando encontra explicitamente uma acao do tipo `Navegar`.

Na pratica, isso significa que o contrato pode declarar uma URL inicial valida e, ainda assim, a automacao comecar a executar passos sobre uma pagina em branco quando a receita nao inclui um passo manual de navegacao.

## Problema

Hoje existe uma inconsistencia entre o significado do contrato e o comportamento do motor:

- o contrato raiz declara `UrlInicial`
- o motor nao usa essa URL automaticamente no bootstrap da execucao
- a receita precisa conhecer esse detalhe tecnico e inserir uma navegacao manual para que o fluxo funcione

Esse comportamento aumenta a fragilidade da receita e favorece falhas silenciosas de configuracao.

## Objetivo da Feature

Fazer com que o motor de automacao abra automaticamente `contrato.UrlInicial` no inicio da execucao, antes de processar as etapas declaradas na receita.

## Resultado Esperado

A aplicacao deve:

1. abrir a URL inicial do contrato assim que a execucao comecar
2. iniciar as etapas somente apos a navegacao inicial ter sido disparada com sucesso
3. manter o suporte atual a acoes `Navegar` para transicoes adicionais ao longo do fluxo
4. reduzir a necessidade de passos de bootstrap repetitivos na receita

## Escopo Funcional

Esta feature deve cobrir:

- uso automatico de `UrlInicial` no inicio da execucao do contrato
- preservacao das acoes `Navegar` ja existentes para navegacoes futuras
- logs claros sobre a URL inicial usada no bootstrap
- falha explicita quando o contrato exigir uma URL inicial e ela estiver ausente ou invalida

## Fora de Escopo

Esta feature nao cobre:

- remocao do tipo de acao `Navegar`
- reestruturacao do contrato JSON
- alteracoes no orquestrador de faturamento
- revisao dos seletores do portal
- suporte a multiplas URLs iniciais concorrentes

## Requisitos Funcionais

- O motor deve navegar automaticamente para `contrato.UrlInicial` antes da execucao das etapas.
- A navegacao inicial deve acontecer apenas uma vez por execucao do contrato.
- A ausencia de `UrlInicial` deve resultar em falha clara quando a receita depender de uma pagina inicial externa.
- Acoes `Navegar` declaradas nas etapas devem continuar funcionando normalmente.
- O log da execucao deve permitir identificar a URL usada no bootstrap.

## Requisitos Tecnicos

- A implementacao deve permanecer concentrada no `ContractBasedAutomationEngine`.
- O comportamento deve preservar o modelo assincrono ja adotado pelo projeto.
- A mudanca nao deve exigir que todas as receitas existentes sejam reescritas.
- A inicializacao automatica deve acontecer antes da iteracao das etapas do contrato.

## Justificativa da Abordagem

Essa abordagem e a mais coerente com o modelo atual porque:

- `UrlInicial` ja existe como parte do contrato raiz
- a ideia de "pagina inicial do fluxo" pertence naturalmente ao bootstrap do motor
- a receita fica menos verbosa e menos sujeita a erro humano
- o tipo `Navegar` continua reservado para mudancas de pagina que acontecem no meio do processo

## Beneficios Esperados

- menor acoplamento da receita a detalhes de bootstrap
- menor chance de esquecer um passo inicial obrigatorio
- contrato mais coerente com seu significado semantico
- diagnostico mais simples quando a automacao falhar logo no inicio

## Criterios de Aceite

- Ao iniciar a execucao, o motor abre automaticamente a `UrlInicial` informada no contrato.
- A receita continua funcionando sem precisar adicionar um passo `Navegar` apenas para entrar na primeira pagina.
- Falhas posteriores passam a refletir o estado real da pagina inicial, e nao a ausencia de navegacao.
- Acoes `Navegar` ainda podem ser usadas no restante do fluxo sem regressao.

## Evidencia que Motivou a Feature

- O contrato `receita_paulistana.json` ja declara `UrlInicial`.
- O motor atual somente chama `GotoAsync` quando encontra uma acao `Navegar`.
- O teste recente indicou que o fluxo opcional de login foi tratado corretamente, mas os seletores da tela principal nao apareceram.
- A explicacao mais provavel e que a automacao nao estava na pagina esperada ao iniciar os passos.

## Arquivos Provavelmente Impactados em Implementacao Futura

- `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- possivelmente `specs/`
- opcionalmente `receita_paulistana.json` se quiserem simplificar a receita depois da mudanca

## Perguntas em Aberto

- A navegacao inicial deve usar apenas `GotoAsync(contrato.UrlInicial)` ou tambem aguardar um estado de carregamento adicional?
- O motor deve aceitar contratos sem `UrlInicial` em cenarios futuros totalmente dirigidos por passos `Navegar`?
- Vale registrar explicitamente um evento de log separado para "bootstrap de navegacao inicial"?
