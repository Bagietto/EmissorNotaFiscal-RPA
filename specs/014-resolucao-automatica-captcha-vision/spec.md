# Feature Specification: Resolução Automática de Captcha com Vision AI

**Feature Branch**: `014-resolucao-automatica-captcha-vision`  
**Created**: 2026-07-07  
**Status**: Draft  
**Input**: User description: "Resolução Automática de Captcha com Vision AI no portal PMSP Auth usando uma API compatível com OpenAI (Google Gemini / OpenRouter)"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Resolução Automática do Captcha de Login (Priority: P1)

Como um operador da automação (Worker), eu quero que o robô consiga resolver o desafio de captcha do portal PMSP Auth de forma autônoma sem supervisão humana, para que o fluxo de emissão de NFS-e possa rodar em segundo plano sem interrupções.

**Why this priority**: É a funcionalidade central da feature. Sem ela, o robô depende inteiramente do operador físico no Modo Assistido quando o portal exige validação humana.

**Independent Test**: Pode ser testado headlessly gerando falhas intencionais de login no PMSP Auth (inserindo credenciais incorretas repetidamente) até que a tela de captcha seja exibida. O robô deve detectar a tela de captcha, capturar o elemento de imagem, chamar a API de Vision (Gemini), preencher o campo e submeter o formulário de desafio com sucesso.

**Acceptance Scenarios**:

1. **Dado** que o portal PMSP Auth apresenta a tela de captcha,  
   **Quando** o resolvedor automático de captcha envia a imagem para o Gemini API e recebe os caracteres corretos,  
   **Então** o robô deve preencher `input#ans`, clicar em `button#jar` e avançar para o fluxo normal de login.

2. **Dado** que a IA respondeu com um valor incorreto e o portal recarregou um novo captcha,  
   **Quando** a quantidade de tentativas restantes for maior que zero,  
   **Então** o robô deve tirar um novo screenshot do elemento de imagem do captcha e submeter uma nova requisição para a API.

3. **Dado** que todas as tentativas de resolução falharam e a contagem atingiu o limite de `MaxRetries`,  
   **Quando** `AssistedMode.Enabled` estiver ativado como `true`,  
   **Então** o robô deve pausar e abrir o Modo Assistido para intervenção manual.  
   **Caso contrário** (`AssistedMode.Enabled` for `false`), o robô deve disparar uma `AutomationExecutionException` descritiva e salvar capturas de diagnóstico.

---

### User Story 2 - Resiliência e Tratamento de Erros da API de Visão (Priority: P2)

Como desenvolvedor da automação, quero que o sistema trate de forma robusta falhas de infraestrutura da API de Visão (como falha de rede, chaves inválidas, timeout ou cotas estouradas), para evitar travamento indefinido do Worker.

**Why this priority**: Evita que indisponibilidades do provedor de IA travem a execução do faturamento geral.

**Independent Test**: Simular falhas de rede no endpoint da API de Visão ou configurar uma API Key inválida e validar se o robô segue a estratégia de fallback configurada.

**Acceptance Scenarios**:

1. **Dado** que a chamada HTTP para o resolvedor de captcha falha com erro de cota (HTTP 429) ou erro interno (HTTP 500),  
   **Quando** o erro persistir após o timeout,  
   **Então** o robô deve registrar a falha de integração em log e prosseguir imediatamente para o fallback (Modo Assistido ou Exception).

2. **Dado** que a API de Visão retorne uma resposta vazia ou em formato inesperado (não-alfanumérico),  
   **Quando** a resposta for recebida pelo resolvedor,  
   **Então** o robô deve tratar isso como uma tentativa incorreta de resolução, atualizar o captcha clicando em `a#bottle` e tentar novamente.

---

## Edge Cases

- **Múltiplos Captchas Consecutivos**: Se o portal exigir um segundo desafio imediatamente após o primeiro ser resolvido (desafio encadeado). O robô deve detectar novamente a presença de captcha usando o mesmo loop de verificação e reiniciar as tentativas.
- **Formatação de Resposta Suja**: Se a IA retornar texto adicional (ex: "Aqui está o texto: A1B2" em vez de apenas "A1B2"). O sistema deve filtrar o texto recebido de forma a extrair estritamente os caracteres alfanuméricos ou usar instruções rígidas de sistema no prompt da chamada.
- **Mudança Súbita de URL**: Se o portal mudar a sessão ou expirar enquanto o captcha está sendo processado pela IA. O robô deve detectar a perda de contexto e reiniciar o fluxo a partir da navegação inicial.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema deve monitorar a visibilidade dos seletores `img:not(#bottle img)` e `input#ans` no domínio `pmspauth.prefeitura.sp.gov.br` para determinar se a tela de captcha está ativa.
- **FR-002**: O sistema deve realizar a captura (screenshot de elemento) exclusivamente do seletor da imagem do captcha e convertê-la em formato Base64.
- **FR-003**: O sistema deve enviar uma requisição HTTP POST para o endpoint configurado em `BaseUrl` contendo a imagem em Base64 e o prompt de instruções estruturado.
- **FR-004**: O sistema deve extrair de forma estrita o texto do captcha resolvido a partir do JSON de resposta da API (compatível com OpenAI `/chat/completions`).
- **FR-005**: O sistema deve preencher a resposta no seletor `input#ans` e clicar no seletor de envio `button#jar` para submeter.
- **FR-006**: O sistema deve disparar o clique de atualização em `a#bottle` se for necessário recarregar o captcha devido a falha pré-envio ou resposta de IA inválida.
- **FR-007**: O sistema deve ser parametrizado dinamicamente através do arquivo `appsettings.json` sob o nó `Automation:CaptchaSolver`.

### Key Entities

- **CaptchaSolverOptions**: Classe de configuração contendo os parâmetros de controle (`Enabled`, `BaseUrl`, `ApiKey`, `Model`, `MaxRetries`, `TimeoutSeconds`, `Selectors`).
- **CaptchaRequestPayload**: Estrutura de dados que modela a carga útil da API de Visão (mensagem de usuário contendo texto e URL de imagem base64).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: O resolvedor automático deve obter taxa de sucesso acumulada de pelo menos **85%** na resolução de captchas (dentro das 3 tentativas máximas).
- **SC-002**: O tempo médio de resposta da API de Visão para o processamento de imagem e texto deve ser menor que **5 segundos**.
- **SC-003**: Em caso de falha de conexão com a API, a automação deve desviar para o fluxo de fallback em menos de **10 segundos** pós-timeout.

## Assumptions

- O usuário possui uma API Key válida do Google Gemini (AI Studio) ou OpenRouter configurada.
- O endpoint de API configurado em `BaseUrl` suporta o formato de requisição `/chat/completions` compatível com a biblioteca padrão de Chat da OpenAI.
- O navegador da automação tem acesso irrestrito à internet para realizar chamadas externas às APIs de IA.
