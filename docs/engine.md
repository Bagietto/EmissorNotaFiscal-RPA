# ⚙️ Contract-Driven Automation Engine (CDAE) - Internals

Este documento descreve o funcionamento interno, o mecanismo de binding dinâmico e as capacidades técnicas da engine de execução da **Contract-Driven Automation Engine (CDAE)**.

---

## 1. Funcionamento Baseado em Contratos

A engine opera interpretando um grafo sequencial de etapas de execução. O motor é agnóstico em relação à regra de negócio do faturamento; ele apenas atua como um executor de instruções atômicas mapeadas.

O processamento baseia-se em duas entradas fornecidas em tempo de execução:
1.  **O Contrato (`FluxoAutomacaoContrato`)**: Contém a lista estruturada de etapas ordenadas e ações estáticas e dinâmicas (seletores HTML).
2.  **O Payload Dinâmico (`Dictionary<string, string>`)**: Um mapeamento chave-valor de dados em tempo de execução que substitui os placeholders contidos no contrato.

### Mecanismo de Data-Binding Dinâmico

Ao atingir uma ação com a propriedade `ValorDinamicoChave` definida, a engine realiza uma resolução tardia (late binding) de valores:

```
  Passo no Contrato:
  {
    "Descricao": "Preencher CNPJ do Cliente",
    "PlaywrightAcao": "PreencherTexto",
    "SeletorHtml": "#ctl00_body_tbCPFCNPJTomador",
    "ValorDinamicoChave": "CnpjCliente"
  }
  
  + Dicionário de Dados:
  { "CnpjCliente": "12345678000199" }
  
  = Execução Física (Playwright):
  await page.Locator("#ctl00_body_tbCPFCNPJTomador").FillAsync("12345678000199");
```

Isso garante que dados variáveis de negócio e credenciais sensíveis permaneçam na memória volátil, sem misturar-se com o arquivo de definição estrutural da interface do portal (o JSON do contrato).

---

## 2. Primitivas de Ação do Motor

O motor traduz as instruções declarativas do JSON utilizando a API nativa do **Microsoft Playwright**. Os seguintes tipos de ação (`TipoAcao`) são suportados nativamente pela engine:

| Primitiva (`TipoAcao`) | Comportamento Técnico no Motor |
| :--- | :--- |
| `Navegar` | Acessa um endereço web absoluto com base na URL configurada ou resolvida dinamicamente. |
| `PreencherTexto` | Foca no seletor HTML correspondente e insere o texto dinâmico associado à chave de dados. |
| `ClicarBotao` | Efetua o clique em um elemento DOM após garantir visibilidade e estabilização de tela. |
| `ClicarSeExistir` | Executa cliques em seletores transientes (como avisos, popups ou banners) usando timeouts curtos. |
| `DispararBlur` | Retira o foco do elemento para disparar eventos assíncronos de validação JavaScript nativos da página. |
| `SelecionarDropdown` | Seleciona opções em menus do tipo HTML `<select>` usando a chave dinâmica fornecida. |
| `ClicarBotaoEAguardarPopup` | Executa o clique e inicia a escuta imediata de novos contextos de página (`IPage`) gerados. |
| `ClicarLinkContendoDinamico` | Localiza e clica em um link `<a>` cujo atributo `href` contenha o valor dinâmico da execução. |
| `DispararDownload` | Inicia a escuta de eventos de download e clica no controle HTML para capturar o stream de arquivo. |
| `AguardarCarregamento` | Pausa a execução até que o estado de tráfego de rede fique ocioso (`NetworkIdle`). |
| `TratarDialogos` | Escuta e responde afirmativamente a popups nativos de alerta JavaScript (`window.alert`, `confirm`). |

---

## 3. Resiliência Cognitiva (Vision AI)

Quando a engine detecta a presença de desafios de Captcha na URL de autenticação do portal, o motor CDAE executa as seguintes etapas estruturadas de processamento visual:

1.  **Isolamento de Elemento**: O motor localiza as coordenadas exatas da imagem do Captcha através do seletor estrutural configurado.
2.  **Screenshot do Elemento**: O Playwright realiza um screenshot isolado apenas da caixa do elemento da imagem, exportando-o em formato binário de memória (PNG).
3.  **Análise Cognitiva (Gemini VLM)**: O arquivo PNG é convertido para uma string Base64 e submetido para a API de visão artificial em um loop com retentativas configuráveis.
4.  **Injeção de Resposta**: O motor preenche o campo de resposta correspondente e submete o formulário.
5.  **Verificação de Continuidade**: A engine analisa a página resultante para validar se o login obteve sucesso ou se a imagem do captcha mudou (indicando erro), permitindo recarga automática da imagem antes de gastar uma nova tentativa.

---

## 4. Fallback Híbrido Assistido (Intervenção Humana)

Se as tentativas de resolução de Captcha via Vision AI falharem consecutivamente ou a funcionalidade estiver desativada nas configurações lógicas:

1.  **Desvio para Modo Visível**: A engine abre o navegador Chromium com interface gráfica (`Headless: false`).
2.  **Alerta e Suspensão de Timeout**: O sistema registra mensagens de atenção no log operacional e suspende o tempo limite convencional de processamento, adotando um intervalo específico para intervenção (padrão: 10 minutos).
3.  **Detecção de Sucesso Traseiro**: O motor monitora periodicamente em segundo plano se o Captcha foi resolvido pelo operador e se os elementos HTML da próxima etapa sequencial do contrato surgiram no DOM.
4.  **Retomada Transparente**: Assim que o operador efetua a entrada do captcha e avança, a engine reassume o fluxo de processamento de forma inteiramente automatizada.

---

## 5. Roadmap de Engenharia (Evolução do Motor)

As seguintes expansões evolutivas estão mapeadas para o desenvolvimento do motor CDAE:

*   **Grafos de Decisão Condicionais**: Evolução do interpretador para suportar estruturas condicionais (`if/else` e ramificações baseadas em elementos do DOM detectados em tempo real).
*   **Barramento de Mensageria Distribuído**: Integração com RabbitMQ ou Azure Service Bus para consumo centralizado de tarefas de faturamento por múltiplos nós CDAE em paralelo.
*   **Plugins e Custom Middleware Hooks**: Capacidade de injetar trechos personalizados de código C# pré ou pós-etapas lógicas do contrato (ex: criptografia de inputs ou integrações externas de APIs).
*   **Observabilidade Centralizada**: Exportação completa de traces OTLP no padrão do OpenTelemetry para ferramentas como Jaeger e Elastic APM.
*   **Dashboard Web Unificado**: Painel administrativo para acompanhamento de telas sob intervenção e publicação de versões de receitas de contratos.
