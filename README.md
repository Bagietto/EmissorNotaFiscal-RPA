# ⚙️ Contract-Driven Automation Engine (CDAE)

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blueviolet.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Playwright](https://img.shields.io/badge/Playwright-Chromium-blue.svg)](https://playwright.dev/dotnet/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%7C%20DDD-brightgreen.svg)]()
[![Observability](https://img.shields.io/badge/Observability-OpenTelemetry-orange.svg)]()

Um motor de automação web resiliente, orientado a contratos declarativos em .NET 8 com Microsoft Playwright e Inteligência Computacional Aplicada.

---

## 1. Visão Geral

Este repositório apresenta a **Contract-Driven Automation Engine (CDAE)**, uma engine de execução projetada para interpretar e operacionalizar fluxos de automação web complexos definidos dinamicamente em tempo de execução via contratos JSON. A premissa central de design do projeto é a dissociação completa entre a inteligência de manipulação do navegador (infraestrutura) e as regras que descrevem a jornada do usuário (negócio/processo).

---

## 📖 Documentação Técnica Detalhada

Para manter este arquivo conciso e focado no guia rápido de inicialização, a documentação de engenharia e decisões arquiteturais foi dividida em guias de leitura dedicados:

*   **[Documento de Arquitetura e Decisões de Projeto](docs/architecture.md)**: Detalha a motivação do design, princípios de separação de responsabilidades (Clean Architecture/DDD), uso de telemetria produtiva nativa e justificativa da stack tecnológica.
*   **[Funcionamento Interno do Motor (CDAE - Internals)](docs/engine.md)**: Explica como o grafo de etapas em JSON é interpretado, as primitivas DOM suportadas, o mecanismo de late binding e o tratamento híbrido de Captcha (Vision AI + Modo Assistido Humano).
*   **[Primeiro Caso de Uso: Emissão de NFS-e Paulistana](docs/use_case_nfse.md)**: Apresenta a lógica do caso prático utilizado para testar a engine, diagramas Mermaid do ciclo de processamento e justificativas de integração.

---

## 🏗️ Estrutura do Projeto

O repositório está organizado sob os princípios da Arquitetura Limpa (Clean Architecture), garantindo testabilidade e separação lógica:

```text
EmissorNotaFiscal/
│
├── EmissorNotaFiscal.sln                    # Arquivo de Solução (.NET)
├── appsettings.json                         # Configurações de infraestrutura do robô e chaves de IA
├── receita_paulistana.json                  # Contrato descritivo de etapas (JSON)
├── config_notas_v2.json                     # Payload e agendamentos lógicos de emissão
│
├── docs/                                    # Manuais de engenharia detalhados
│   ├── architecture.md                      # Decisões de arquitetura e stack tecnológica
│   ├── engine.md                            # Internals do motor de execução
│   └── use_case_nfse.md                     # Caso de uso real de NFS-e Paulistana
│
├── src/
│   └── EmissorNotaFiscal/                   # Projeto Principal (Worker Service)
│       ├── Program.cs                       # Inicialização e Injeção de Dependências (DI)
│       ├── Worker.cs                        # BackgroundService que gerencia a periodicidade das execuções
│       ├── Application/                     # Orquestrador de fluxo de negócio (FaturamentoOrchestrator)
│       ├── Configuration/                   # Classes de mapeamento de opções fortemente tipadas
│       ├── Domain/                          # Núcleo declarativo do sistema (Modelos e Interfaces)
│       └── Infrastructure/                  # Implementações de tecnologias concretas (Playwright, JSON)
│
└── tests/
    └── EmissorNotaFiscal.Tests/             # Projeto de testes de unidade e mock (xUnit / NSubstitute)
```

---

## 🔧 Configuração e Preparação

### 1. Parâmetros Globais do Motor (`appsettings.json`)

Configure os recursos de infraestrutura e comportamento cognitivo da engine.

```json
{
  "Automation": {
    "Headless": true,
    "DownloadsDirectory": "F:\\Projetos\\AI\\playwright\\EmissorNotaFiscal\\downloads",
    "AssistedMode": {
      "Enabled": true,
      "HumanInterventionTimeoutMinutes": 10
    },
    "CaptchaSolver": {
      "Enabled": true,
      "BaseUrl": "https://generativelanguage.googleapis.com/v1beta/openai/chat/completions",
      "ApiKey": "SUA_API_KEY_DO_GEMINI_AQUI",
      "Model": "gemini-2.5-flash",
      "MaxRetries": 3,
      "TimeoutSeconds": 15,
      "Selectors": {
        "CaptchaImage": "img:not(#bottle img)",
        "InputResponse": "input#ans",
        "SubmitButton": "button#jar",
        "ReloadButton": "a#bottle"
      }
    }
  }
}
```

### 2. Payload de Execução de Negócio (`config_notas_v2.json`)

Defina os dados dinâmicos específicos das faturas que serão aplicados aos contratos lógicos do motor.

```json
{
  "configuracoes_emissor": {
    "cnpj_prestador": "00000000000000",
    "codigo_servico_paulistana": "02660"
  },
  "agendamento_notas": [
    {
      "cnpj_cliente": "12345678000199",
      "razao_social_placeholder": "Razao Social Cliente LTDA",
      "email_cliente": "cliente@example.com",
      "valor_nota": 1500.00,
      "dia_emissao": 5,
      "descricao_personalizada": "Consultoria e desenvolvimento de sistemas de automação de processos baseados em contratos.",
      "ultima_emissao": ""
    }
  ]
}
```

---

## 🚀 Como Executar

### Pré-requisitos
1.  **SDK do .NET 8.0** instalado na máquina.
2.  PowerShell ou terminal Bash moderno.

### Inicialização Passo a Passo

1.  **Restaurar as dependências do projeto**:
    ```bash
    dotnet restore
    ```

2.  **Instalar os navegadores do Playwright**:
    Compile o projeto para gerar os executáveis locais do Playwright e em seguida instale as instâncias de navegadores requeridas:
    ```bash
    dotnet build
    
    # Executar a instalação do driver de browser do Chromium
    pwsh src/EmissorNotaFiscal/bin/Debug/net8.0/playwright.ps1 install chromium
    ```

3.  **Executar o Worker**:
    ```bash
    dotnet run --project src/EmissorNotaFiscal/EmissorNotaFiscal.csproj
    ```

---

## 🔒 Segurança em Produção

> 🛑 **Importante**: Nunca armazene credenciais de acesso ou chaves de API cruas nos arquivos JSON de configuração dentro do controle de versão Git.

Para implantações profissionais, a injeção da variável sensitiva `Automation:SenhaWeb` deve ser configurada utilizando os provedores nativos de configuração do .NET:
*   **Ambiente de Desenvolvimento**: Utilize o gerenciador de segredos locais do .NET (`dotnet user-secrets`).
*   **Ambiente de Produção**: Injete chaves através de **Variáveis de Ambiente** (`Environment Variables`) mapeadas no servidor host ou utilize cofres de chaves dedicados (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault).

---
*Este repositório foi desenvolvido sob conceitos de engenharia de software de nível corporativo para servir como demonstração de competências em arquitetura orientada a dados, resiliência de processos distribuídos e desenvolvimento moderno em ecossistemas .NET.*
