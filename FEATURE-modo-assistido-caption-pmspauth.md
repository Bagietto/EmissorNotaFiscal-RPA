# Feature: Modo Assistido para Caption no PMSP Auth

## Contexto

Depois dos ajustes no fluxo de Login Unico, a automacao passou a conseguir chegar ate a autenticacao e, em algumas execucoes, avancar para a area autenticada do portal.

Durante a investigacao, tambem foi identificado que o dominio de autenticacao `pmspauth.prefeitura.sp.gov.br` pode apresentar uma tela de challenge/caption controlada pelo provedor.

Essa tela nao aparece de forma deterministica em todas as execucoes, mas quando surge ela interrompe o fluxo automatico normal.

## Problema Real Encontrado

O portal de autenticacao pode interpor uma tela de validacao humana antes de liberar a continuidade do login.

Na investigacao realizada, foram encontrados indicios concretos de uma pagina com:

- imagem do challenge
- audio do challenge
- campo de resposta manual
- botao de submissao
- scripts de protecao do provedor

Isso significa que, neste momento, a automacao nao pode assumir que sempre saira da etapa de autenticacao apenas com preenchimento de usuario e senha.

## Conclusao

Para uma primeira etapa, a solucao mais coerente nao e tentar resolver automaticamente o caption.

Uma abordagem totalmente automatica agora traria alto risco tecnico, por depender de OCR, heuristicas visuais, interpretacao de audio ou tentativas mais frageis diante de uma protecao de terceiros.

A estrategia mais segura e valiosa neste momento e um modo assistido:

- o worker roda com o navegador aberto
- quando a tela de caption aparecer, a automacao pausa
- o usuario preenche e submete o challenge manualmente
- o worker aguarda ate que o portal realmente avance
- apos o acesso ser restabelecido, o fluxo automatico retoma os passos seguintes

## Objetivo da Feature

Permitir que a automacao continue operavel mesmo quando o portal exigir intervencao humana no caption, sem perder o restante do fluxo automatizado.

## Resultado Esperado

A automacao deve:

1. executar o fluxo em modo visual quando o modo assistido estiver habilitado
2. detectar que caiu em uma tela de challenge/caption
3. pausar a progressao automatica enquanto o challenge estiver presente
4. informar em log que esta aguardando intervencao humana
5. permitir que o operador resolva manualmente o caption no navegador aberto
6. retomar automaticamente quando o portal sair do challenge e atingir um estado valido de continuidade

## Abordagem Recomendada

Nesta primeira fase, a feature deve adotar explicitamente um modo assistido.

Isso significa:

- nao implementar OCR
- nao tentar reconhecer automaticamente o texto da imagem
- nao tentar resolver via audio
- nao tentar contornar a protecao do provedor

O foco e apenas detectar o bloqueio, aguardar o operador e retomar com seguranca quando o fluxo voltar ao caminho esperado.

## Escopo Funcional

Esta feature deve cobrir:

- habilitacao de execucao com navegador visivel no fluxo assistido
- deteccao da tela de challenge/caption por elementos estruturais reais
- pausa controlada da automacao enquanto o challenge estiver presente
- mensagem clara de log indicando espera por acao humana
- retomada automatica apos validacao de que o portal realmente saiu do challenge
- timeout configuravel para a espera humana

## Fora de Escopo

Esta feature nao cobre:

- leitura automatica da imagem do caption
- OCR ou reconhecimento de audio
- servicos externos de solucao de captcha
- bypass de mecanismos anti-bot
- automacao completa sem operador
- revisao de outros seletores do portal fora do contexto do challenge

## Estados que Precisam Ser Diferenciados

Para a feature ser robusta, o fluxo deve diferenciar pelo menos estes cenarios:

1. tela normal de login
2. tela de challenge/caption
3. erro ou pagina inesperada no `pmspauth`
4. retorno para area autenticada do NFS-e
5. disponibilidade real do menu para seguir o fluxo

## Recomendacao de Retomada

O worker nao deve retomar apenas porque a pagina mudou.

Ele deve voltar ao fluxo somente quando houver um indicio funcional claro de sucesso, como:

- saida da tela de challenge
- retorno a uma pagina valida do fluxo autenticado
- disponibilidade do menu ou de elementos que comprovem a continuidade real

## Criterios de Aceite

- O fluxo deve conseguir permanecer em espera quando a tela de caption for exibida.
- O navegador deve permanecer aberto para permitir a intervencao manual do operador.
- O log deve informar claramente que a automacao esta aguardando resolucao humana do challenge.
- A automacao nao deve falhar imediatamente ao detectar a tela de caption.
- Depois que o operador submeter o caption corretamente, o worker deve retomar sozinho o fluxo.
- A retomada deve ocorrer apenas quando o portal atingir um estado valido de continuidade.
- Se a resolucao manual nao ocorrer dentro do prazo definido, o fluxo deve encerrar com mensagem clara de timeout.

## Beneficios da Abordagem

- reduz o risco tecnico da primeira entrega
- preserva o valor do restante da automacao
- permite operar o fluxo real imediatamente
- gera observabilidade para uma futura fase automatica
- evita investimento precoce em OCR ou heuristicas frageis

## Riscos

- dependencia de operador presente no momento da execucao
- timeout caso o usuario nao responda a tempo
- possibilidade de erro humano ao digitar o caption
- possibilidade de o portal apresentar mais de um challenge na mesma sessao
- variacao futura no formato da tela de challenge
- risco de falsos positivos se a deteccao nao for baseada em elementos estruturais confiaveis

## Perguntas em Aberto

- Qual sera o melhor conjunto de seletores para detectar a tela de challenge com seguranca?
- Qual tempo limite faz mais sentido para a espera do operador?
- O modo assistido sera configurado por `appsettings`, por argumento de linha de comando, ou pelos dois?
- A deteccao de retomada deve usar URL, elementos do DOM, ou ambos?
- Como registrar de forma clara quando o operador resolveu errado e o challenge reapareceu?
- Vale persistir evidencias diagnosticas quando o timeout expirar?

## Arquivos Provavelmente Impactados em Implementacao Futura

- `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- `appsettings.json`
- `Configuration/`
- opcionalmente `receita_paulistana.json`
- opcionalmente `specs/`
