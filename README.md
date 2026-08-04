# ApiUsuarios - Auth Service

API ASP.NET Core para cadastro, login e consulta de usuarios. A aplicacao esta organizada para atuar como o servico de identidade/autenticacao de outros servicos.

## Rotas principais

- `POST /api/auth/register`: cadastra usuario.
- `POST /api/auth/login`: autentica usuario e retorna JWT.
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

## Executar com Docker Compose

```powershell
docker compose up --build
```

A API fica disponivel em:

```text
http://localhost:8080
```
