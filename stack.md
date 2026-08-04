# Stack do Projeto

Este documento registra a stack alvo para a arquitetura de backend, frontend e infraestrutura dos micro-servicos do projeto.

## Visao geral

A arquitetura sera orientada a micro-servicos, com foco em APIs de backend independentes, comunicacao por HTTP e mensageria assincrona, conteinerizacao com Docker e separacao clara de responsabilidades usando Clean Architecture e principios SOLID.

## Backend

- Plataforma: .NET 10 LTS.
- Linguagem: C# 14.
- Framework web: ASP.NET Core.
- Tipo de aplicacao: APIs REST para micro-servicos.
- Autenticacao: JWT Bearer Token.
- Autorizacao: claims, roles e policies quando necessario.
- Persistencia: SQL Server com Entity Framework Core quando houver banco relacional.
- Documentacao de API: Swagger/OpenAPI.
- Health checks: endpoints de saude por servico.

## Arquitetura

O backend deve seguir Clean Architecture, separando regras de negocio, casos de uso, infraestrutura e interface externa.

Estrutura recomendada por micro-servico:

```text
src/
  ServiceName.Api/
  ServiceName.Application/
  ServiceName.Domain/
  ServiceName.Infrastructure/
tests/
  ServiceName.UnitTests/
  ServiceName.IntegrationTests/
```

Responsabilidades:

- `Api`: controllers, endpoints, middlewares, filtros, autenticacao, Swagger e configuracao HTTP.
- `Application`: casos de uso, DTOs, validacoes, contratos de servicos e orquestracao.
- `Domain`: entidades, value objects, regras de dominio, eventos de dominio e interfaces essenciais.
- `Infrastructure`: banco de dados, mensageria, providers externos, implementacoes de repositorios e configuracoes tecnicas.
- `Tests`: testes unitarios, testes de integracao e validacao dos contratos principais.

## Principios de desenvolvimento

- Aplicar principios SOLID.
- Manter regras de negocio fora de controllers.
- Preferir injecao de dependencia por interfaces nos limites entre camadas.
- Evitar acoplamento direto entre micro-servicos.
- Preferir contratos explicitos para APIs e mensagens.
- Manter endpoints pequenos e services/casos de uso coesos.
- Tratar erros com respostas padronizadas.
- Validar entradas antes de executar regras de negocio.
- Versionar migrations e contratos relevantes.

## Autenticacao e autorizacao

- Usar JWT Bearer Token para autenticar chamadas entre cliente e APIs.
- Access tokens devem ter vida curta.
- Refresh tokens devem ser armazenados com hash no banco quando utilizados.
- Roles como `Admin` e `User` devem ser constantes ou tipos centralizados, evitando strings soltas.
- Rotas administrativas devem usar policies ou roles explicitamente.
- Segredos de JWT nao devem ficar fixos no codigo-fonte em ambientes reais.

## Micro-servicos

Cada micro-servico deve ter responsabilidade bem definida e banco proprio quando fizer sentido.

Diretrizes:

- Um servico nao deve acessar diretamente o banco de outro.
- Comunicacao sincrona via HTTP deve ser usada para consultas ou operacoes que exigem resposta imediata.
- Comunicacao assincrona via RabbitMQ deve ser usada para eventos, processamento desacoplado e integracoes entre servicos.
- Cada servico deve expor health check.
- Cada servico deve poder rodar isoladamente via Docker.

## Banco de dados

- Banco relacional atual da aplicacao: SQL Server.
- ORM: Entity Framework Core.
- Migrations: EF Core Migrations versionadas no repositorio.
- Connection string local atual: `Server=localhost\sqlexpress; Initial Catalog=ApiUsuarios; Integrated Security=True; TrustServerCertificate=True`.
- Em Docker/local integrado, SQL Server deve ser configurado via `docker-compose.yml` quando o servico precisar subir banco junto com a API.
- Em ambientes reais, connection strings e credenciais devem vir de variaveis de ambiente, secret manager ou cofre de segredos.

Diretrizes:

- Cada micro-servico deve possuir seu proprio banco ou schema quando houver necessidade de persistencia isolada.
- Um micro-servico nao deve acessar diretamente tabelas de outro micro-servico.
- Integracoes entre dados de servicos diferentes devem ocorrer via APIs ou eventos RabbitMQ.
- Migrations devem ser aplicadas de forma controlada por ambiente.

## Mensageria

- Broker: RabbitMQ.
- Padrao preferido: eventos assincronos entre micro-servicos.
- Publicadores nao devem depender da implementacao interna dos consumidores.
- Mensagens devem ter contratos versionaveis.
- Consumidores devem ser idempotentes sempre que possivel.
- Filas devem considerar retry, dead-letter queue e observabilidade.

Exemplos de eventos:

- `UserRegistered`
- `UserUpdated`
- `UserDeleted`
- `AuthTokenRefreshed`

## Docker

- Cada micro-servico deve ter seu proprio `Dockerfile`.
- O ambiente local deve usar `docker-compose.yml` para subir APIs, bancos e RabbitMQ.
- Imagens devem usar multi-stage build.
- Configuracoes sensiveis devem vir por variaveis de ambiente ou secret manager.
- Containers devem expor portas somente quando necessario.

## Frontend

- Framework: React.
- Linguagens/base web: HTML, CSS e JavaScript/TypeScript.
- Consumo de APIs: HTTP/REST.
- Autenticacao no cliente: uso de JWT retornado pelo backend.
- Responsabilidade do frontend: interface, experiencia do usuario, validacoes de formulario e integracao com APIs.

Diretrizes:

- Separar componentes de UI, paginas, hooks e servicos de API.
- Centralizar configuracao de cliente HTTP.
- Tratar expiracao de token e renovacao com refresh token.
- Evitar expor segredos no frontend.

## Observabilidade e operacao

- Logs estruturados por servico.
- Health checks para readiness/liveness.
- Correlation ID para rastrear requisicoes entre micro-servicos.
- Metricas basicas de HTTP, banco e filas.
- Monitoramento de falhas em consumidores RabbitMQ.

## Testes

- Testes unitarios para regras de dominio e casos de uso.
- Testes de integracao para banco, autenticacao e endpoints principais.
- Testes de contrato para mensagens RabbitMQ quando houver integracao entre servicos.
- Testes de API para fluxos criticos como cadastro, login, refresh token e autorizacao.

## Decisoes atuais

- A stack alvo para novos servicos sera .NET 10 LTS com C# 14.
- .NET 11 e C# 15 nao serao alvo principal enquanto estiverem em preview.
- O backend sera organizado em Clean Architecture.
- A comunicacao entre servicos deve priorizar eventos com RabbitMQ quando houver desacoplamento real.
- O frontend sera React com HTML/CSS e integracao via APIs REST.

## Referencias oficiais

- .NET downloads: https://dotnet.microsoft.com/download/dotnet
- Politica de suporte do .NET: https://dotnet.microsoft.com/platform/support/policy/dotnet-core
- Novidades do .NET 10: https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10/overview
- Novidades do C# 14: https://learn.microsoft.com/dotnet/csharp/whats-new/csharp-14
