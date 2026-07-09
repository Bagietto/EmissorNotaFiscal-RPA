# Feature: Pós-Emissão - Consulta de Confirmação e Download do PDF

## Contexto

Após a consolidação da rotina de emissão de NFS-e (Nota Fiscal de Serviços Eletrônica) no portal da Prefeitura de São Paulo (NFS-e Paulistana), o objetivo principal passou a ser a **confirmação e obtenção do PDF oficial pós-emissão**.

Em vez de realizarmos downloads históricos ou de períodos antigos, o fluxo funcionará como uma **etapa de encerramento da própria emissão**. Imediatamente após clicar em emitir a nota fiscal, a automação fará o seguinte:
1. Navegará para a tela de Consulta de Notas.
2. Filtrará pelo **CPF/CNPJ do Tomador (Cliente)** e pelo **mês/ano corrente**.
3. Abrirá os resultados da consulta (que surgem em uma janela popup).
4. Localizará e abrirá a nota fiscal recém-emitida.
5. Efetuará o download do PDF oficial através do botão mapeado `input#btDownload`.

Este método garante a validação de que a nota foi efetivamente gravada pelo portal e recupera o arquivo em formato e layout idênticos aos gerados manualmente.

---

## Descobertas e Mapeamento do Fluxo Técnico

Durante a simulação assistida, identificamos os seguintes comportamentos e estruturas no portal relevantes para a pós-emissão:

1. **Bypass de Bot e Autenticação (PMSP Auth)**:
   - A automação de login no domínio `pmspauth.prefeitura.sp.gov.br` deve ser executada com o canal oficial do Google Chrome (`Channel = "chrome"` no Playwright), ocultando a propriedade de automação (`--disable-blink-features=AutomationControlled` nos argumentos) e utilizando um User-Agent real para evitar a redefinição de conexão (`ERR_CONNECTION_RESET`).
   - O formulário tradicional de Senha Web deve ser acessado expandindo sequencialmente:
     1. A aba "Outras formas de acesso" (`button.divider-accordion-toggle`).
     2. A aba "Senha Web" (`button.login-accordion-header` ou `button:has-text("Senha Web")`).

2. **Filtro de Consulta por Tomador (Cliente)**:
   - Após finalizar a emissão, a automação navega para `https://nfe.prefeitura.sp.gov.br/contribuinte/consultas.aspx` (ou clica no menu `a[href*="consultas.aspx"]`).
   - O filtro será direcionado para buscar a nota específica:
     - **CNPJ/CPF do Tomador**: Preenchido no input `#ctl00_body_tbCPFCNPJ` utilizando a chave dinâmica `CnpjCliente`.
     - **Ano**: Selecionado no dropdown `select#ctl00_body_ddlExercicio` com o ano da emissão (ex: `2026`).
     - **Mês**: Selecionado no dropdown `select#ctl00_body_ddlMes` com o mês da emissão (ex: `7`).
     - **Gatilho**: Clique em `input#ctl00_body_btEmitidas` (NFS-e EMITIDAS) que submete a consulta e **abre uma nova janela popup**.

3. **Leitura de Resultados (Popup)**:
   - A URL da janela popup gerada segue o padrão:  
     `https://nfe.prefeitura.sp.gov.br/contribuinte/notasapuradas.aspx?inscricao=...&cpfcnpj=[CnpjCliente]&ano=[Ano]&mes=[Mes]...`
   - Na tabela de resultados, localizamos o link `a` correspondente à nota. O texto do link é o número da nota fiscal emitido (apenas dígitos, ex: `00000141`) e o href aponta para:  
     `notaprint.aspx?inscricao=...&nf=[numero_nota]&verificacao=[codigo_verificacao]`.

4. **Página de Detalhe e Download do PDF**:
   - O clique no link da nota abre a página de visualização impressa em `https://nfe.prefeitura.sp.gov.br/contribuinte/notaprint.aspx?...`.
   - No topo desta página, localiza-se o botão oficial de download:  
     `input#btDownload` (ou seletor `input[value="Download NFS-e"]`).
   - O clique no botão inicia o download do arquivo PDF no navegador.

---

## Mapeamento Técnico de Seletores e URLs

Abaixo estão os seletores exatos e URLs mapeados para a implementação das etapas de consulta pós-emissão:

| Passo do Fluxo | Ação | Elemento / Seletor HTML | URL de Contexto | Observação / Comportamento |
| :--- | :--- | :--- | :--- | :--- |
| **Navegar p/ Consulta** | Navegar / Clique | `a[href*="consultas.aspx"]` | `/contribuinte/principal.aspx` | Acessa a tela de filtros de pesquisa. |
| **Preencher CPF/CNPJ** | Preencher | `input#ctl00_body_tbCPFCNPJ` | `/contribuinte/consultas.aspx` | Filtra pelo CNPJ do Cliente (`CnpjCliente`). |
| **Selecionar Ano** | Selecionar | `select#ctl00_body_ddlExercicio` | `/contribuinte/consultas.aspx` | Dropdown do ano da nota. |
| **Selecionar Mês** | Selecionar | `select#ctl00_body_ddlMes` | `/contribuinte/consultas.aspx` | Dropdown do mês da nota. |
| **Pesquisar Emitidas** | Clicar | `input#ctl00_body_btEmitidas` | `/contribuinte/consultas.aspx` | **Abre nova janela popup** com a lista. |
| **Selecionar Nota** | Clicar | `a[href*="notaprint.aspx"]` | `/contribuinte/notasapuradas.aspx` | Link numérico da nota fiscal. |
| **Download PDF** | Clicar | `input#btDownload` | `/contribuinte/notaprint.aspx` | **Dispara download** do arquivo PDF. |

---

## Integração na `receita_paulistana.json`

Podemos estender a receita atual adicionando as etapas de consulta e download logo após a etapa de **Finalização da Emissão**:

```json
    {
      "Ordem": 5,
      "NomeEtapa": "Finalizacao da Emissao",
      "Acoes": [
        {
          "Descricao": "Disparar Emissao Final da Nota",
          "PlaywrightAcao": "ClicarBotao",
          "SeletorHtml": "#ctl00_body_btEmitir, input[id*='btEmitir' i], input[name*='btEmitir' i]"
        },
        {
          "Descricao": "Aguardar Processamento Final da Emissao",
          "PlaywrightAcao": "AguardarCarregamento"
        }
      ]
    },
    {
      "Ordem": 6,
      "NomeEtapa": "Navegacao para Consulta de Confirmacao",
      "Acoes": [
        {
          "Descricao": "Acessar menu Consulta de Notas",
          "PlaywrightAcao": "ClicarBotao",
          "SeletorHtml": "#ctl00_wpMenuLateral_mnuRotinasn4[href=\"consultas.aspx\"], a[href*=\"consultas.aspx\"]"
        },
        {
          "Descricao": "Aguardar carregamento da pagina de filtros",
          "PlaywrightAcao": "AguardarCarregamento"
        }
      ]
    },
    {
      "Ordem": 7,
      "NomeEtapa": "Filtro pelo Tomador e Periodo",
      "Acoes": [
        {
          "Descricao": "Preencher CNPJ do Cliente",
          "PlaywrightAcao": "PreencherTexto",
          "SeletorHtml": "#ctl00_body_tbCPFCNPJ",
          "ValorDinamicoChave": "CnpjCliente"
        },
        {
          "Descricao": "Selecionar Ano da Emissao",
          "PlaywrightAcao": "SelecionarDropdown",
          "SeletorHtml": "#ctl00_body_ddlExercicio",
          "ValorDinamicoChave": "AnoEmissao"
        },
        {
          "Descricao": "Selecionar Mes da Emissao",
          "PlaywrightAcao": "SelecionarDropdown",
          "SeletorHtml": "#ctl00_body_ddlMes",
          "ValorDinamicoChave": "MesEmissao"
        },
        {
          "Descricao": "Clicar Consultar Emitidas",
          "PlaywrightAcao": "ClicarBotaoEAAguardarPopup",
          "SeletorHtml": "#ctl00_body_btEmitidas"
        }
      ]
    },
    {
      "Ordem": 8,
      "NomeEtapa": "Abertura e Download do PDF",
      "Acoes": [
        {
          "Descricao": "Clicar na Nota Recem-Emitida",
          "PlaywrightAcao": "ClicarLinkContendoDinamico",
          "SeletorHtml": "a[href*=\"notaprint.aspx\"]",
          "ValorDinamicoChave": "NumeroNota"
        },
        {
          "Descricao": "Baixar PDF da Nota",
          "PlaywrightAcao": "DispararDownload",
          "SeletorHtml": "input#btDownload"
        }
      ]
    }
```

## Requisitos de Configuração e Diretório de Download

Para garantir a confiabilidade operacional e evitar que downloads falhem silenciosamente ou em etapas tardias do processo, a automação deve seguir regras rígidas de validação do diretório de destino:

1. **Configuração Centralizada**:
   - O caminho do diretório de salvamento deve ser lido das configurações da aplicação (`appsettings.json`), sob a chave `Automation:DownloadsDirectory`.

2. **Validação Antecipada (Fail-Fast)**:
   - **Primeiro passo da execução**: Antes de iniciar o Playwright ou instanciar o navegador, o motor de automação deve verificar se o diretório de destino existe.
   - Caso não exista, o motor deve tentar criá-lo imediatamente utilizando as APIs nativas do sistema de arquivos (`Directory.CreateDirectory`).

3. **Tratamento de Restrições e Permissões**:
   - Se ocorrer qualquer exceção durante a criação ou teste de escrita (como `UnauthorizedAccessException` devido a falta de permissões ou `IOException` por caminhos inválidos):
     - A exceção deve ser capturada e convertida em uma mensagem de erro detalhada e amigável ao usuário.
     - Deve ser lançado um erro descritivo (ex: `AutomationConfigurationException`) contendo:
       - O caminho absoluto do diretório que falhou.
       - A causa provável (ex: falta de permissão de escrita ou caminho de rede inacessível).
       - A mensagem técnica interna original para fins de diagnóstico.
     - O processo de automação deve abortar imediatamente neste ponto, impedindo o login ou preenchimento de dados inúteis.

---

## Padrões de Arquitetura e Tecnologias

A implementação desta feature deve seguir rigorosamente as diretrizes e stacks estabelecidas para o projeto, evitando códigos obsoletos:

1. **Stack e Linguagem**:
   - Desenvolvido em **C# / .NET 8.0**.
   - Utilização das APIs assíncronas nativas da linguagem (`async/await`) em todas as operações de I/O de rede e disco.

2. **Injeção de Dependências e Configuração**:
   - O caminho e configurações do diretório devem ser mapeados via padrão Options do .NET (`IOptions<T>`) injetado no construtor dos serviços, mantendo consistência com o restante do projeto.

3. **Playwright Moderno**:
   - Evitar o uso de seletores baseados em XPath complexos e difíceis de ler. Priorizar seletores baseados em IDs (`#id`), classes CSS estáveis, ou localizadores de acessibilidade recomendados pela documentação atualizada do Playwright.
   - Tratar abas/popups usando as APIs nativas de eventos assíncronos (`context.WaitForPageAsync`) evitando esperas forçadas (`Thread.Sleep`) ou lógicas obsoletas de manipulação de janelas.

4. **Isolamento de Camadas**:
   - A lógica do navegador e interação DOM devem permanecer restritas à camada de **Infrastructure** (`Infrastructure/Automation`), enquanto os contratos e interfaces residem em **Domain**, respeitando a Clean Architecture do projeto.
