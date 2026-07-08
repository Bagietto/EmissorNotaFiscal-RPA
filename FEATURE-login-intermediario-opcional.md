# Feature: Suporte a Fluxo Opcional de Login Intermediario

## Contexto

Durante o teste real da automacao do portal da NFS-e Paulistana, foi identificado que a pagina inicial pode apresentar um fluxo intermediario com o botao `Login unico` antes da tela tradicional que contem os campos `#cpfCnpj` e `#password`.

Esse comportamento nao e deterministico: em algumas execucoes a tela intermediaria aparece, e em outras o portal pode abrir diretamente na tela de credenciais.

No estado atual, a aplicacao executa a receita de automacao como um fluxo estritamente linear. Por isso, quando a tela intermediaria aparece, a automacao falha ao tentar localizar `#cpfCnpj` antes que esse campo exista na tela ativa.

## Problema

O motor atual nao suporta variacoes condicionais no fluxo de navegacao.

Hoje a execucao:

1. segue exatamente a ordem definida em `receita_paulistana.json`
2. exige que cada seletor exista no momento esperado
3. nao possui mecanismo de passo opcional, fallback ou desvio condicional

Como consequencia:

- se a tela intermediaria aparecer, a automacao falha cedo demais
- se a receita for alterada para sempre clicar em `Login unico`, a automacao tambem pode falhar quando essa tela nao aparecer

## Objetivo da Feature

Permitir que a automacao trate corretamente a presenca opcional da tela intermediaria de login, sem quebrar o fluxo quando ela nao aparecer.

## Resultado Esperado

A aplicacao deve conseguir:

1. detectar se a tela intermediaria de `Login unico` esta presente
2. interagir com ela quando necessario
3. seguir diretamente para o preenchimento do login tradicional quando essa etapa nao existir
4. manter a receita de automacao legivel e previsivel
5. preservar logs claros para diagnostico do caminho seguido em cada execucao

## Escopo Funcional

Esta feature deve cobrir:

- suporte a acao opcional baseada em seletor
- suporte a variacao de fluxo sem quebrar a execucao principal
- adaptacao da receita `receita_paulistana.json` para representar esse comportamento
- logs que indiquem se o caminho com `Login unico` foi usado ou ignorado

## Fora de Escopo

Esta feature nao cobre:

- redesenho completo do motor de automacao
- suporte generico a workflows complexos com multiplos ramos arbitrarios
- mudancas no modelo de faturamento
- alteracoes no envio de email ou pos-processamento da nota
- estabilizacao de todos os seletores do portal alem do necessario para esse fluxo

## Requisitos Funcionais

- O motor deve permitir ao menos uma operacao opcional, que nao encerre a execucao se o seletor nao estiver presente.
- A receita deve conseguir representar o passo de entrada pelo `Login unico` sem obrigar esse clique em todos os cenarios.
- O fluxo deve continuar buscando `#cpfCnpj` e `#password` apos tratar ou ignorar a tela intermediaria.
- A execucao deve registrar em log qual caminho foi seguido.
- Falhas reais apos a decisao de fluxo devem continuar sendo propagadas normalmente.

## Requisitos Tecnicos

- A implementacao deve preservar o modelo assincrono atual.
- A orquestracao deve continuar desacoplada do Playwright concreto por meio de `INfeAutomationService`.
- A evolucao deve se concentrar no motor de automacao e na receita JSON, evitando espalhar regras do portal por outras camadas.
- A solucao deve ser compativel com o fluxo atual de execucao do `Worker`.

## Possiveis Abordagens

### Opcao 1: Nova acao opcional no motor

Adicionar suporte a uma acao como `ClicarSeExistir`, `AguardarSeExistir` ou equivalente.

Vantagens:

- mudanca pequena e focada
- simples de representar na receita
- resolve o problema atual com baixo acoplamento

Riscos:

- pode nao cobrir necessidades futuras mais complexas

### Opcao 2: Suporte a etapa condicional por seletor

Adicionar uma estrutura de decisao baseada na presenca de um seletor.

Vantagens:

- modelagem mais explicita do fluxo
- melhor base para futuras variacoes

Riscos:

- aumenta a complexidade do contrato JSON e do motor

## Recomendacao Inicial

Comecar pela Opcao 1, com suporte a um passo opcional orientado por seletor, porque ela resolve o comportamento observado agora com o menor impacto estrutural.

Se surgirem novas variacoes relevantes no portal, essa capacidade pode depois evoluir para um modelo mais rico de condicoes.

## Criterios de Aceite

- A automacao nao deve falhar apenas porque a tela `Login unico` apareceu antes da tela de credenciais.
- A automacao nao deve falhar apenas porque a tela `Login unico` nao apareceu.
- A receita deve continuar permitindo chegar ao preenchimento de `#cpfCnpj`.
- Os logs devem mostrar se o passo opcional foi executado ou ignorado.
- O restante das falhas deve continuar rastreavel com contexto de etapa e acao.

## Evidencia que Motivou a Feature

- O teste real selecionou corretamente o item de faturamento e chegou ao motor de automacao.
- A execucao falhou aguardando o seletor `#cpfCnpj`.
- Foi observado manualmente que o portal pode exibir antes uma tela com o botao `Login unico`.
- Essa variacao nao esta contemplada no contrato atual da receita nem nas capacidades atuais do motor.

## Arquivos Provavelmente Impactados em Implementacao Futura

- `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- `Domain/Models/Automation/TipoAcao.cs`
- `Domain/Models/Automation/AcaoPasso.cs`
- `receita_paulistana.json`
- possivelmente documentacao em `specs/`

## Perguntas em Aberto

- O seletor mais estavel para a tela intermediaria sera `button.oauth-button`, outro seletor estrutural, ou texto visivel?
- O clique opcional deve apenas tentar uma vez ou aguardar por uma janela curta antes de seguir?
- O mesmo padrao opcional sera util para outras telas transitarias do portal, como banners ou confirmacoes?
