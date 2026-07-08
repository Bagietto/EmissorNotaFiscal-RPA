# Research: Ajuste de Encoding e Seletor do Menu de Emissao

## Decision 1: Trocar o locator textual por um seletor estrutural composto

- **Decision**: Atualizar `Acessar Menu Lateral Emissao` para `#ctl00_wpMenuLateral_mnuRotinasn3[href="nota.aspx"], #ctl00_wpMenuLateral_mnuRotinasn3 a[href="nota.aspx"]`.
- **Rationale**: A investigacao ja aponta dois sinais estruturais confiaveis do alvo real: o identificador `ctl00_wpMenuLateral_mnuRotinasn3` e o destino `nota.aspx`. O seletor composto cobre tanto o caso em que o id esteja no proprio link quanto o caso em que ele esteja no container do item.
- **Alternatives considered**:
  - `text=Emissao de NFS-e`: descartado por fragilidade a acentos, encoding e variacoes cosmeticas.
  - `a[href="nota.aspx"]` isolado: viavel, mas menos restritivo se houver outro link para o mesmo destino em outro trecho da pagina.

## Decision 2: Manter o motor de automacao inalterado

- **Decision**: Nao alterar `Infrastructure/Automation/ContractBasedAutomationEngine.cs` nesta iteracao.
- **Rationale**: O motor ja sabe aguardar e clicar elementos visiveis; a falha observada esta no alvo configurado na receita.

## Decision 3: Registrar a causa ligada a encoding sem transformar isso em logica de parsing

- **Decision**: Documentar explicitamente que o ajuste e uma estrategia de resiliencia a `iso-8859-1`, mas resolver o problema por seletor estrutural e nao por tratamento especial de encoding no engine.
- **Rationale**: O encoding afeta a confiabilidade do texto visivel, mas nao exige processamento adicional quando o DOM ja oferece identificadores estruturais melhores.
