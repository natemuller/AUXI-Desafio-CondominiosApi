# AutenticacaoApi

Emite o token JWT (Bearer) usado por `CondominiosApi`, `BlocosApi` e `UnidadesApi`. Não tem endpoints de consulta — sua única responsabilidade é login.

- **Porta (dev):** `http://localhost:5038` / `https://localhost:7257`
- **Swagger:** `/swagger`

## POST /api/auth/login

Autentica por CPF ou e-mail e devolve um token JWT válido por `Jwt:ExpirationMinutes` (60 min por padrão).

**Autenticação:** nenhuma (`AllowAnonymous`).

### Request body

```json
{
  "cpfOuEmail": "<cpf ou e-mail cadastrado, ou o valor de DevCredential:Cpf em ambiente de dev>",
  "senha": "<senha correspondente, ou DevCredential:Password em ambiente de dev>"
}
```

| Campo | Tipo | Obrigatório | Observação |
|---|---|---|---|
| `cpfOuEmail` | string | sim | CPF (só dígitos) ou e-mail cadastrado em `usuarios` |
| `senha` | string | sim | Senha em texto puro |

### Como a senha é validada

1. **Atalho de desenvolvimento** (`DevCredential:*`, configurado via `user-secrets`): se `DevCredential:Enabled=true` e `cpfOuEmail`/`senha` baterem exatamente com `DevCredential:Cpf`/`DevCredential:Password`, o login é aceito direto — desde que exista em `usuarios` um registro com esse CPF. Pensado só para permitir gerar um token rapidamente em ambiente de desenvolvimento, sem depender do hash real salvo no banco.
2. **Caminho real**: senha comparada via `BCrypt.Net.BCrypt.Verify` contra `usuario_credenciais.senha_hash`.

> Fora de escopo (deliberado): rotação de refresh token (`usuario_sessoes`), bloqueio por tentativas falhas (`tentativas_falhas`/`bloqueado_ate`) e claims de papel/role (`papeis`, `usuario_condominio_papeis`) não são aplicados nesta versão.

### Resposta 200 OK

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresInSeconds": 3600,
  "tokenType": "Bearer",
  "usuario": {
    "id": "b6d1c1e0-0000-0000-0000-000000000000",
    "cpf": "12345678900",
    "nomeCompleto": "Usuário de Teste",
    "email": "usuario@exemplo.com"
  }
}
```

### Resposta 401 Unauthorized

```json
{ "message": "CPF/e-mail ou senha inválidos." }
```

Motivos: usuário não encontrado, ou senha não confere em nenhum dos dois caminhos acima.

## Usando o token nas outras APIs

Copie o valor de `accessToken` e cole no botão **Authorize** do Swagger de `CondominiosApi`/`BlocosApi`/`UnidadesApi` (formato `Bearer <token>`), ou envie no header em qualquer cliente HTTP:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

Todas as três validam o mesmo token (mesma `Jwt:SigningKey`, `Issuer` e `Audience`) — não é preciso logar de novo em cada uma.
