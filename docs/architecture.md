# 🏗️ Arquitetura e Decisões de Projeto (CDAE)

Este documento detalha a arquitetura de software, a divisão de componentes e as principais decisões de engenharia adotadas na **Contract-Driven Automation Engine (CDAE)**.

---

## 1. Motivação da Arquitetura

A arquitetura do projeto foi estruturada com base nas seguintes prioridades de engenharia de software corporativo:

*   **Motor Desacoplado do Domínio**: A engine expõe primitivas lógicas de interação de tela (Preencher, Clicar, Selecionar, Baixar) sem guardar conhecimento funcional sobre as regras fiscais de faturamento. Ela apenas processa a receita descrita no contrato JSON fornecido.
*   **Declaração Dinâmica em JSON**: Toda a jornada de navegação e seletores reside em um grafo sequencial serializado em arquivos JSON. Isso viabiliza o versionamento de jornadas via Git e facilita o diagnóstico rápido de quebras de DOM.
*   **Configuração vs. Codificação**: Ajustes operacionais de seletores e tempos limites são feitos via injeção de parâmetros nas receitas estruturadas, diminuindo custos de manutenção.
*   **Separação Estrita de Responsabilidades (DDD & Clean Architecture)**: As definições de domínio do faturamento residem livres de dependências tecnológicas, enquanto a manipulação física de infraestrutura baseia-se no Playwright.

---

## 2. Decisões Arquiteturais Justificadas

As escolhas técnicas na estruturação do repositório sustentam-se em justificativas de engenharia voltadas para ambientes estáveis de produção contínua:

### A. Worker Service Pattern
A hospedagem do motor em um `BackgroundService` assíncrono nativo do .NET permite a execução contínua em servidores corporativos. O processo integra-se ao ciclo de vida do sistema operacional host, facilitando a conteinização via Docker e implantações em contêineres Linux sob orquestração de nuvem (como Kubernetes ou AWS ECS).

### B. Clean Architecture & DDD Simplificado
O isolamento de regras de negócios contra os detalhes de automação do browser foi garantido pela segregação estrita de camadas.
*   *Justificativa*: Se o portal de destino for substituído por uma API gRPC ou REST no futuro, a lógica de faturamento, agendamentos e orquestração (`Application`/`Domain`) não mudará. Apenas a camada de `Infrastructure` será substituída por um serviço HTTP Client convencional.

### C. JSON Contracts (Engine Dirigida por Metadados)
Toda a interação com o DOM é definida em um arquivo JSON estático (`receita_paulistana.json`).
*   *Justificativa*: A manutenção de RPAs tradicionais é extremamente cara devido a mudanças frequentes na interface de terceiros. Ao externalizar os seletores estruturais e a ordem dos passos, o robô pode ser consertado ou expandido com alterações simples no arquivo JSON do contrato, sem exigir re-compilação de código e novos pipelines de build/deploy.

### D. Vision AI + Fallback Híbrido Assistido
Integração de resolvedores cognitivos com salvaguardas humanas operacionais sob timeouts controlados.
*   *Justificativa*: Soluções de RPA convencionais quebram imediatamente ao se depararem com Captchas. Esta engine implementa um fluxo em camadas (Tiered Approach): tenta resolver autonomamente usando LLMs/VLMs visuais eficientes; caso falhe, abre o navegador visivelmente no monitor do servidor para intervenção humana rápida.

### E. Telemetria e Monitoramento Produtivo
Uso de contadores de métricas e atividades do .NET (`ActivitySource` e `Meter`) incorporados no código do Worker para instrumentação.
*   *Justificativa*: Habilita a exportação direta de sinais de execução (como ciclos de faturamento efetuados e erros de configuração detectados) para agregadores APM modernos como Prometheus, Datadog ou Elastic Search via coletores de OpenTelemetry.

---

## 3. Divisão de Responsabilidades (Componentização)

O repositório adota uma organização lógica que reflete o isolamento de camadas:

*   **Domain**: Define as regras e abstrações lógicas básicas. A estrutura do contrato de automação (`FluxoAutomacaoContrato`, `AcaoPasso`, `TipoAcao`) e as interfaces puras de serviço (`INfeAutomationService`, `IConfigRepository`) residem aqui, sem qualquer dependência tecnológica externa.
*   **Infrastructure (Automation)**: Responsável por traduzir o contrato abstrato do domínio em instruções físicas no navegador usando a API do Microsoft Playwright. Contém o resolvedor cognitivo de Captcha via HTTP.
*   **Application**: O orquestrador (`FaturamentoOrchestrator`) conecta o domínio e a infraestrutura. Ele carrega as configurações lógicas de notas, o contrato JSON, monta o dicionário de dados dinâmicos e invoca a engine de automação.
*   **Worker**: Gerencia o loop de execução periódica em segundo plano no host do sistema operacional.

---

## 4. Stack Tecnológica Justificada

*   **C# / .NET 8.0**: Escolhido pela alta eficiência em processamento assíncrono, tipagem forte que elimina erros de mapeamento de contratos, injeção de dependência nativa de alto nível e ciclo de vida robusto.
*   **Microsoft Playwright**: Selecionado em detrimento de ferramentas antigas (Selenium/Puppeteer) por seu suporte avançado a esperas inteligentes implícitas (auto-waiting), facilidade de tratamento de abas secundárias/popups e interceptação simplificada de fluxos de download assíncronos.
*   **OpenAI/Gemini Vision API (VLM)**: Integração via chat completions compatível com visão computacional para decodificação OCR de desafios em imagem, eliminando o custo de licenças comerciais de quebra de captcha.
*   **System.Diagnostics (Metrics & Tracing)**: Utilização de telemetria baseada no próprio runtime da plataforma .NET, permitindo integração direta com agentes de infraestrutura.
