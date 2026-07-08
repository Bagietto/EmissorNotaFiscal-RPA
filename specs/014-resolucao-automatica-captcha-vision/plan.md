# Implementation Plan: Resolução Automática de Captcha com Vision AI

**Branch**: `014-resolucao-automatica-captcha-vision` | **Date**: 2026-07-07 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/014-resolucao-automatica-captcha-vision/spec.md`

## Summary

Implementar um resolvedor automático de captcha de imagem integrado no fluxo de autenticação do portal PMSP Auth. O resolvedor utilizará visão computacional chamando uma API compatível com OpenAI (como Google Gemini no AI Studio ou OpenRouter) utilizando parâmetros dinâmicos definidos no arquivo de configuração `appsettings.json`. Em caso de falha persistente ou de infraestrutura, a execução deve desviar de forma segura para o Modo Assistido existente ou encerrar com erro descritivo.

## Technical Context

**Language/Version**: C# / .NET 8.0  
**Primary Dependencies**: Microsoft.Playwright, Microsoft.Extensions.Logging, Microsoft.Extensions.Configuration, Microsoft.Extensions.Options, System.Net.Http, System.Text.Json  
**Storage**: N/A (Persistência em memória e configuração local via `appsettings.json`)  
**Testing**: MSTest / NUnit (para testes de unidade do Resolvedor) e validação manual headlessly disparando captcha  
**Target Platform**: Windows Worker Process  
**Project Type**: Background Worker Service  
**Performance Goals**: Tempo total de processamento e envio à API de Vision < 5 segundos por tentativa  
**Constraints**: 
- Não introduzir dependências pesadas externas (utilizar `System.Net.Http` nativo e `System.Text.Json` para parsing).
- Permitir configuração flexível de qualquer endpoint compatível com OpenAI `/chat/completions` (permitindo uso imediato do Gemini API gratuito do Google AI Studio).
- Isolar a lógica de chamada de API em um serviço separado (`ICaptchaSolverService`) para facilitar a testabilidade.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Check | Status | Notes |
|---|---|---|
| A arquitetura é flexível a múltiplos provedores? | PASS | Configurável via `BaseUrl` e `Model` no `appsettings.json`. |
| O engine Playwright continua isolado? | PASS | O resolvedor realiza apenas chamadas HTTP de rede puras, sem interagir com instâncias do navegador. |
| O Modo Assistido atual é preservado? | PASS | Sim, atua como o principal fallback em caso de falha persistente de Vision AI. |
| Tratamento de dados sensíveis (API Key)? | PASS | A API Key será mantida nas configurações e pode ser injetada via variáveis de ambiente. |

## Project Structure

### Documentation (this feature)

```text
specs/014-resolucao-automatica-captcha-vision/
├── plan.md              # This file
├── checklists/
│   └── requirements.md  # Requirements validation
└── tasks.md             # Tasks list (dependency ordered)
```

### Source Code

```text
appsettings.json
Program.cs
Configuration/
├── AssistedAutomationOptions.cs
└── CaptchaSolverOptions.cs            # [NEW] Opções do resolvedor de captcha
Domain/
├── Interfaces/
│   ├── IConfigRepository.cs
│   ├── INfeAutomationService.cs
│   └── ICaptchaSolverService.cs        # [NEW] Contrato do resolvedor
Infrastructure/
├── Automation/
│   ├── ContractBasedAutomationEngine.cs # Integrar resolvedor no loop de login
│   └── OpenAiCompatibleCaptchaSolver.cs # [NEW] Implementação do resolvedor
```

**Structure Decision**: Criar uma interface separada `ICaptchaSolverService` no Domain para desacoplar a chamada HTTP externa do motor Playwright (`ContractBasedAutomationEngine`). A injeção de dependência injetará a implementação concreta `OpenAiCompatibleCaptchaSolver`.

## Proposed Changes

### Configuration Layer

#### [NEW] [CaptchaSolverOptions.cs](file:///f:/Projetos/AI/playwright/EmissorNotaFiscal/Configuration/CaptchaSolverOptions.cs)
Classe que mapeia as configurações da seção `Automation:CaptchaSolver` do `appsettings.json`:
* `Enabled` (bool)
* `BaseUrl` (string, URL completa do endpoint `/chat/completions`)
* `ApiKey` (string)
* `Model` (string, ex: `gemini-2.5-flash` ou `google/gemini-2.5-flash`)
* `MaxRetries` (int)
* `TimeoutSeconds` (int)
* `Selectors` (objeto `CaptchaSelectors` contendo `CaptchaImage`, `InputResponse`, `SubmitButton`, `ReloadButton`)

#### [MODIFY] [appsettings.json](file:///f:/Projetos/AI/playwright/EmissorNotaFiscal/appsettings.json)
Inserir o nó `"CaptchaSolver"` sob o nó `"Automation"` com os seletores reais da tela mapeados na simulação.

#### [MODIFY] [Program.cs](file:///f:/Projetos/AI/playwright/EmissorNotaFiscal/Program.cs)
Registrar as opções do `CaptchaSolverOptions` no container de injeção de dependência e registrar as classes `HttpClient` necessárias.

### Domain Layer

#### [NEW] [ICaptchaSolverService.cs](file:///f:/Projetos/AI/playwright/EmissorNotaFiscal/Domain/Interfaces/ICaptchaSolverService.cs)
Interface simples que recebe a imagem e retorna a string contendo a solução do captcha:
```csharp
namespace EmissorNotaFiscal.Domain.Interfaces;

public interface ICaptchaSolverService
{
    Task<string> SolveAsync(string base64Image, CancellationToken cancellationToken = default);
}
```

### Infrastructure Layer

#### [NEW] [OpenAiCompatibleCaptchaSolver.cs](file:///f:/Projetos/AI/playwright/EmissorNotaFiscal/Infrastructure/Automation/OpenAiCompatibleCaptchaSolver.cs)
Implementação de `ICaptchaSolverService` utilizando `HttpClient`:
* Envia um payload JSON compatível com a API de Chat Completions da OpenAI (contendo texto e imagem base64).
* Insere o cabeçalho `Authorization: Bearer <ApiKey>` e `HTTP-Referer`.
* Trata a resposta JSON, extraindo `choices[0].message.content`.
* Realiza uma limpeza básica na resposta para retornar estritamente caracteres alfanuméricos sem espaços adicionais.

#### [MODIFY] [ContractBasedAutomationEngine.cs](file:///f:/Projetos/AI/playwright/EmissorNotaFiscal/Infrastructure/Automation/ContractBasedAutomationEngine.cs)
* Injetar `ICaptchaSolverService` e `IOptions<CaptchaSolverOptions>`.
* No método `AguardarModoAssistidoSeNecessarioAsync` (ou em um método anterior no fluxo):
  * Verificar se o solver está habilitado e se a tela de captcha está visível.
  * Executar o loop de tentativas até `MaxRetries`:
    1. Tirar screenshot do elemento de imagem do captcha.
    2. Enviar a imagem para o `ICaptchaSolverService.SolveAsync`.
    3. Digitar a resposta no seletor `InputResponse` (`input#ans`).
    4. Clicar em `SubmitButton` (`button#jar`).
    5. Aguardar navegação ou nova verificação de visibilidade do captcha.
    6. Se o captcha continuar visível (ou seja, se a resposta foi incorreta), clicar em `ReloadButton` (`a#bottle`), aguardar curto delay e repetir.
  * Se todas as tentativas de resolução falharem:
    * Se `AssistedMode.Enabled` for `true`, cair no loop manual assistido existente.
    * Caso contrário, lançar `AutomationExecutionException`.

## Verification Plan

### Automated Tests
- Teste de unidade para `OpenAiCompatibleCaptchaSolver` usando mock de `HttpMessageHandler` para verificar o envio correto do payload e tratamento de respostas HTTP com sucesso e falha (429, 500, etc.).
- Verificação de compilação da solução com `dotnet build`.

### Manual Verification
- Rodar a aplicação headlessly disparando a tela de captcha (3 tentativas falhas de credenciais).
- Acompanhar logs detalhados de execução para validar:
  1. A detecção correta da tela de captcha.
  2. O envio correto da payload para o Gemini API.
  3. A digitação da resposta correta no campo `input#ans`.
  4. O clique no botão `button#jar`.
  5. O sucesso da autenticação e prosseguimento do fluxo.
