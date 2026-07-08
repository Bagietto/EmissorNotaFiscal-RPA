# Feature: Finalizacao da Emissao com Confirmacao e Download do PDF

## Contexto

Depois dos ajustes de:

- Login Unico
- navegacao inicial
- seletor do menu de emissao
- modo assistido para challenge/caption

o fluxo passou a avancar bem mais no portal.

Nas execucoes acompanhadas visualmente, a automacao conseguiu:

1. abrir a URL inicial
2. passar pelo Login Unico
3. acessar a area autenticada
4. avancar no fluxo de emissao
5. preencher a descricao do servico e o valor da nota

Mesmo assim, a execucao finalizou com erro.

## Problema Real Encontrado

O erro atual nao indica mais falha de login nem de navegacao.

A investigacao mostrou um desencontro entre:

- o que a receita atualmente faz
- o que o engine espera ao final

### O que a receita faz hoje

A `receita_paulistana.json` termina na etapa 4 com apenas duas acoes:

- preencher `#ctl00_body_tbDiscriminacao`
- preencher `#ctl00_body_tbValor`

Ou seja, nao existe hoje na receita nenhuma acao para:

- clicar no botao de emitir a NF
- tratar a confirmacao final do portal
- acionar a geracao do documento final

### O que o engine espera hoje

O `ContractBasedAutomationEngine` sempre:

1. registra captura de downloads no inicio do fluxo
2. executa todas as etapas da receita
3. ao final, chama obrigatoriamente `ResolveDownloadedPdfPathAsync(...)`
4. falha se nenhum PDF tiver sido baixado em ate 30 segundos

Logo, mesmo que a receita preencha corretamente os campos, o fluxo nunca podera terminar com sucesso sem uma etapa explicita de finalizacao da emissao e sem o gatilho real do download.

## Evidencia da Investigacao

### Evidencia funcional observada

Durante o teste assistido, foi observado que a automacao chegou ate o ponto de:

- preencher descricao do servico
- preencher valor da nota

Mas nao realizou o ato final de emissao.

Tambem foi levantada a expectativa operacional de que, ao emitir, o portal provavelmente exibira uma mensagem de confirmacao perguntando se deseja prosseguir.

### Evidencia estrutural

Na receita atual:

- nao existe passo de clique em botao de emissao
- nao existe passo de confirmacao pos-emissao

No engine atual:

- o tratamento de dialogos existe e aceita automaticamente dialogs do navegador
- mas o retorno do servico depende obrigatoriamente de um PDF baixado

Isso explica o erro reproduzido:

- `A automacao foi concluida sem produzir um caminho de PDF baixado.`

## Conclusao

O fluxo atual esta incompleto do ponto de vista de negocio da emissao.

Em outras palavras:

- a automacao consegue chegar ao formulario
- consegue preencher os campos principais
- mas ainda nao executa a finalizacao da emissao
- sem a finalizacao, nao ha download de PDF
- sem download de PDF, o engine sempre falha no encerramento

## Objetivo da Feature

Completar o fluxo de emissao apos o preenchimento dos campos financeiros, incluindo:

- clique no comando final de emitir a NF
- tratamento da confirmacao exigida pelo portal
- continuidade ate a geracao efetiva do PDF

## Resultado Esperado

A automacao deve:

1. preencher os campos da etapa 4
2. acionar o botao final de emissao
3. aceitar a confirmacao exibida pelo portal, quando houver
4. aguardar a finalizacao do processamento
5. produzir o download real do PDF
6. encerrar o fluxo retornando um caminho de PDF valido

## Recomendacao de Ajuste

Adicionar uma nova etapa de finalizacao apos a etapa de discriminacao e valores.

Essa etapa deve contemplar, no minimo:

- clique no botao de emitir a nota
- espera pela resposta do portal
- confirmacao automatica do dialogo ou alerta, caso ele seja realmente o mecanismo usado
- espera pelo download do PDF ou por algum marcador confiavel que anteceda esse download

## Recomendacao Inicial

Comecar pela receita e pelos seletores do fluxo real de emissao, antes de qualquer mudanca estrutural no engine.

O engine ja possui:

- captura de dialogos
- captura de downloads PDF

Portanto, o gargalo mais provavel agora e a ausencia do trecho final na receita ou a falta de um passo explicito que dispare o processo de emissao.

## Escopo Funcional

Esta feature deve cobrir:

- identificacao do botao real de emissao final
- inclusao de etapa de emissao na `receita_paulistana.json`
- compatibilizacao com a confirmacao exibida pelo portal
- validacao de que o fluxo dispara o download do PDF

## Fora de Escopo

Esta feature nao cobre:

- ajustes no Login Unico
- tratamento de caption/challenge
- revisao de toda a estrategia de download do engine
- interpretacao de erros cadastrais do tomador
- alteracoes no conteudo da nota alem da finalizacao do envio

## Criterios de Aceite

- A automacao deve deixar de encerrar logo apos preencher descricao e valor.
- O fluxo deve clicar no comando real de emissao da NF.
- Se o portal solicitar confirmacao para prosseguir, a automacao deve tratar esse passo corretamente.
- A emissao deve avancar ate a geracao do documento final.
- O engine deve receber um caminho de PDF valido ao final da execucao.
- O erro `A automacao foi concluida sem produzir um caminho de PDF baixado.` nao deve mais ocorrer em execucoes validas.

## Riscos

- o portal pode usar confirmacao em formato diferente de um dialogo nativo
- o botao final pode variar por perfil, permissao ou estado do formulario
- pode existir validacao intermediaria antes do download do PDF
- o download pode ocorrer apenas apos outra navegacao, janela ou callback assicrono

## Perguntas em Aberto

- Qual e o seletor real do botao final de emissao?
- A confirmacao acontece por dialogo nativo do navegador, `alert`, `confirm`, modal HTML ou outro mecanismo?
- O download do PDF acontece imediatamente apos a confirmacao ou existe uma tela intermediaria?
- O fluxo de emissao abre nova aba, nova janela ou permanece na mesma pagina?
- Existe algum requisito adicional de validacao antes da emissao quando o tomador nao possui cadastro completo?

## Arquivos Provavelmente Impactados em Implementacao Futura

- `receita_paulistana.json`
- opcionalmente `Infrastructure/Automation/ContractBasedAutomationEngine.cs`
- opcionalmente `specs/`
