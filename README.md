# ApiUsuarios - Auth Service

API ASP.NET Core para cadastro, login e consulta de usuarios. A aplicacao esta organizada para atuar como o servico de identidade/autenticacao de outros servicos.

## Rotas principais

- `POST /api/auth/register`: cadastra usuario.
- `POST /api/auth/login`: autentica usuario e retorna JWT.
- `GET /api/users`: lista usuarios autenticados.
- `GET /api/users/{id}`: busca usuario autenticado por id.
- `DELETE /api/users/{id}`: remove usuario autenticado por id.
- `GET /health`: health check basico.

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

Configure segredos por variavel de ambiente quando usar fora do ambiente local:

```powershell
$env:JWT_TOKEN = "troque_por_um_token_longo_e_secreto"
$env:SA_PASSWORD = "troque_por_uma_senha_forte"
```

## Observacoes de banco

A migration `RemoveStoredTokenAndAddUserIndexes` remove a coluna `Token` da tabela `Usuarios` e adiciona indices unicos para `Email` e `Usuario`. Antes de aplicar em um banco com dados reais, verifique se nao existem emails ou usuarios duplicados.
