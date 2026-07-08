# Research: Modo Assistido para Caption no PMSP Auth

## Decision 1: Usar modo assistido em vez de automacao do challenge

- **Decision**: Tratar o challenge do `pmspauth` com pausa assistida e intervencao humana.
- **Rationale**: A feature explicitamente descarta OCR, audio e bypass. O objetivo imediato e preservar a operabilidade do restante do fluxo.

## Decision 2: Detectar o challenge por marcadores estruturais ligados a `mcaptcha`

- **Decision**: Usar seletores estruturais relacionados a `mcaptcha`, imagem/audio de captcha e campos de resposta como sinais do challenge.
- **Rationale**: O log local ja mostra a presenca de `mcaptcha.js` e do elemento `mcaptcha__token-label`, o que oferece um ponto de partida mais confiavel do que texto visivel.
- **Alternatives considered**:
  - Detectar apenas por URL do `pmspauth`: descartado por ser amplo demais e incluir estados legitimos sem challenge.
  - Detectar por mensagem textual renderizada: descartado por fragilidade a variacao do provedor.

## Decision 3: Retomar apenas quando houver continuidade funcional

- **Decision**: Retomar somente quando o challenge desaparecer e houver sinais reais de continuidade, como a visibilidade de seletores das etapas seguintes ou a saida do dominio `pmspauth`.
- **Rationale**: Evita retomada precoce em paginas intermediarias ou tentativas humanas malsucedidas.

## Decision 4: Configurar o modo assistido via `appsettings` e linha de comando

- **Decision**: Expor `Automation:AssistedMode:Enabled` e `Automation:AssistedMode:HumanInterventionTimeoutMinutes`.
- **Rationale**: O host ja suporta `appsettings`, variaveis de ambiente e argumentos de linha de comando, entao a feature ganha flexibilidade sem nova infraestrutura.
