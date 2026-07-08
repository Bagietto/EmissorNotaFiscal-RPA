# Data Model: Redirecionamento Browser-Driven do Login Unico

## Entities

### LoginUnicoRedirect

Represents the browser-driven transition that begins after the Login Unico click.

**Attributes**
- `SourcePage` - the page where the click occurs
- `FinalUrl` - destination reached after the redirect
- `Completed` - indicates the redirect finished before continuing

**Rules**
- Must be triggered by the browser interaction, not by a hardcoded navigation.
- Must be logged after completion.

### AuthentictionTargetPage

Represents the final page reached after the redirect where credential fields are expected.

**Attributes**
- `Url` - current address after navigation
- `CredentialSelectors` - selectors expected to exist after redirect

**Rules**
- Must be available before the login credential steps execute.

