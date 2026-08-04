# ApiUsuarios - Auth Service

API ASP.NET Core para cadastro, login e consulta de usuarios. A aplicacao esta organizada para atuar como o servico de identidade/autenticacao de outros servicos.

## Rotas principais

- `POST /api/auth/register`: cadastra usuario.
- `POST /api/auth/login`: autentica usuario e retorna JWT com refresh token.
- `POST /api/auth/refresh-token`: renova JWT e rotaciona o refresh token.
- `POST /api/auth/logout`: revoga o refresh token ativo do usuario autenticado.
- `GET /api/users/me`: busca o usuario autenticado.
- `PUT /api/users/me`: edita o usuario autenticado.
- `DELETE /api/users/me`: remove o usuario autenticado.
- `GET /api/users`: lista usuarios, restrito a perfil `Admin`.
- `GET /api/users/{id}`: busca usuario por id apenas quando for o proprio usuario autenticado.
- `PUT /api/users`: edita usuario apenas quando o id enviado for o proprio usuario autenticado.
- `DELETE /api/users/{id}`: remove usuario por id apenas quando for o proprio usuario autenticado.
- `GET /health`: health check basico.

## Perfis de usuario

Novos usuarios sao cadastrados com perfil `User`. O perfil `Admin` pode acessar rotas administrativas, como `GET /api/users`.

Para definir o primeiro administrador em ambiente local, atualize o usuario diretamente no banco e faca login novamente para receber um JWT com a role atualizada:

```sql
UPDATE Usuarios
SET Role = 'Admin'
WHERE Email = 'junior@example.com';
```

## Tokens

O JWT de acesso expira em 30 minutos por padrao. O refresh token expira em 7 dias, e a API salva apenas o hash dele no banco.

Esses tempos podem ser ajustados em `AppSettings`:

```json
{
  "AccessTokenExpirationMinutes": 30,
  "RefreshTokenExpirationDays": 7
}
```

## Executar localmente

```powershell
dotnet restore
dotnet ef database update
dotnet run
```

Swagger em ambiente de desenvolvimento:

```text
http://localhost:5196/swagger
```

## Executar frontend

```powershell
cd frontend
npm install
npm run dev
```

O frontend fica disponivel em:

```text
http://localhost:5173
```

Configure `VITE_API_BASE_URL` quando precisar apontar para outra API ou futuro BFF. Exemplo em `frontend/.env.example`.

## Executar com Docker Compose

```powershell
docker compose up --build
```

A API fica disponivel em:

```text
http://localhost:8080
```
