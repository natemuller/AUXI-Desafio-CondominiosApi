# AUXI Desafio — Condomínios API

API de consulta de dados de condomínios, desenvolvida como desafio de estágio (Auxiliadora Predial). Três endpoints GET — **Condomínios**, **Blocos** e **Unidades** — protegidos por JWT, com paginação e cache central, sobre uma base Postgres (Supabase) já populada.

## Arquitetura

Um único projeto `Core` (class library) concentra tudo que é compartilhado — persistência (EF Core + Npgsql), repositórios, paginação, cache e autenticação JWT — e é referenciado por 4 Web APIs independentes (Minimal APIs, vertical slice por feature: `Features/<Nome>/{Request,Response,Handler,Endpoint}`):

```
Core/                      # class library compartilhada
├── Models/                # entidades EF (Condominio, Bloco, Unidade, Usuario, ...)
├── Persistence/           # AuxiDbContext, configurations, DependencyInjection.AddCore(...)
├── Repositories/          # um repositório por entidade (paginado)
├── Common/                # PaginationDefaults (10 itens/página), PagedResult<T>
├── Auth/                  # JwtOptions, IJwtTokenService, AddJwtBearerAuthentication(...)
├── Caching/               # CacheOptions, ICacheService, ResponseCacheEndpointFilter
└── Swagger/               # AddSwaggerWithJwtAuth(...) — Swagger com botão Authorize

AutenticacaoApi/           # emite o JWT (login)
CondominiosApi/            # GET /api/condominios (+ /{codCondom})
BlocosApi/                 # GET /api/blocos (+ /{codCondom}/{codBloco})
UnidadesApi/                # GET /api/unidades (+ /{ideconomia})
StartApi/                  # host único: as mesmas 4 APIs acima, numa porta só (ver abaixo)

*.Tests/                   # um projeto xUnit por API
Docs/                      # documentação detalhada de cada API + PDFs do desafio
```

Cada API é um processo/porta independente, mas todas validam o **mesmo** token (mesma chave HS256, issuer e audience) — o login acontece uma única vez, na `AutenticacaoApi`.

`StartApi` (nome do desafio) não duplica código: é um 5º projeto executável que referencia os 4 acima e mapeia todos os endpoints juntos, num único Swagger com uma seção por "entidade" (**Autenticação**, **Condomínios**, **Torres**, **Unidades**) — útil pra testar tudo numa página só, sem trocar de porta. Os 4 projetos originais continuam funcionando isolados normalmente.

## Stack

.NET 10 · Minimal APIs · EF Core + Npgsql (Postgres/Supabase) · JWT Bearer · Swashbuckle (Swagger) · BCrypt.Net-Next · xUnit

## Pré-requisitos

- .NET SDK 10
- Acesso à instância Postgres (Supabase) do projeto

## Configuração (segredos)

Nenhuma credencial fica em `appsettings.json` versionado — tudo via `dotnet user-secrets`. Rode em **cada um** dos 4 projetos web (`AutenticacaoApi`, `CondominiosApi`, `BlocosApi`, `UnidadesApi`):

```bash
dotnet user-secrets init --project <Projeto>
dotnet user-secrets set "ConnectionStrings:SupabaseConnection" "Host=<host>;Port=5432;Database=postgres;Username=<usuario>;Password=<senha>;SSL Mode=Require;Trust Server Certificate=true" --project <Projeto>
dotnet user-secrets set "Jwt:SigningKey" "<mesma chave HS256 nos 4 projetos>" --project <Projeto>
```

Só na `AutenticacaoApi`, para permitir gerar um token de teste sem depender do hash de senha real:

```bash
dotnet user-secrets set "DevCredential:Enabled" "true" --project AutenticacaoApi
dotnet user-secrets set "DevCredential:Cpf" "<cpf de um usuário existente em 'usuarios'>" --project AutenticacaoApi
dotnet user-secrets set "DevCredential:Password" "<senha combinada>" --project AutenticacaoApi
```

> Nota: neste ambiente de desenvolvimento, o host direto do Supabase não resolveu DNS — foi usada a connection string via **pooler** (`aws-1-sa-east-1.pooler.supabase.com`, usuário `postgres.<project-ref>`). Se o host direto (`db.<project-ref>.supabase.co`) resolver na sua rede, ambos funcionam.

`Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpirationMinutes` e `Cache:TtlSeconds` não são segredo — já vêm com valor padrão em `appsettings.json` de cada projeto.

## Como rodar

Também precisa dos mesmos segredos configurados em `StartApi` (mesmos comandos acima, trocando `<Projeto>` por `StartApi`, incluindo `DevCredential:*`).

| Projeto | HTTP | HTTPS | Swagger |
|---|---|---|---|
| `StartApi` **(recomendado — tudo numa página)** | 5080 | 7080 | `/swagger` |
| `AutenticacaoApi` | 5038 | 7257 | `/swagger` |
| `CondominiosApi` | 5216 | 7236 | `/swagger` |
| `BlocosApi` | 5299 | 7299 | `/swagger` |
| `UnidadesApi` | 5399 | 7399 | `/swagger` |

```bash
# opção rápida: um processo só, com todos os endpoints
dotnet run --project StartApi

# ou, como serviços separados:
dotnet run --project AutenticacaoApi
dotnet run --project CondominiosApi
dotnet run --project BlocosApi
dotnet run --project UnidadesApi
```

## Autenticação e teste rápido (Swagger)

1. `POST /api/auth/login` (seção **Autenticação** do Swagger) → copia o `accessToken` da resposta.
2. Clica em **Authorize** (topo da página) e cola o token — vale para as seções Condomínios/Torres/Unidades também, é a mesma página.
3. Chama qualquer um dos endpoints de consulta normalmente.

Rodando as 4 APIs separadas em vez do `StartApi`, o fluxo é o mesmo, só que o login acontece na `AutenticacaoApi` e o token precisa ser colado em cada Swagger individualmente (portas diferentes). Detalhes completos (request/response, exemplos, regras de validação) em [`Docs/autenticacao.md`](Docs/autenticacao.md).

## Endpoints de consulta

| API | Endpoint | Documentação |
|---|---|---|
| Condomínios | `GET /api/condominios` (filtros: `cnpj`, `codCondom`, `nome`) + `GET /api/condominios/{codCondom}` | [`Docs/condominios.md`](Docs/condominios.md) |
| Blocos | `GET /api/blocos` (filtros: `codCondom`, `codBloco`, `descricao`, `ativo`) + `GET /api/blocos/{codCondom}/{codBloco}` | [`Docs/blocos.md`](Docs/blocos.md) |
| Unidades | `GET /api/unidades` (filtros: `codCondom`, `codBloco`, `codEconom`, `tipoUnidade`, `ativa`, `nomeCondomino`) + `GET /api/unidades/{ideconomia}` | [`Docs/unidades.md`](Docs/unidades.md) |

Todos retornam **todas as colunas** das respectivas tabelas (`condominios`, `blocos`, `unidades`) — sem contrato "curado".

## Paginação e cache

- **Paginação**: fixa em 10 itens por página (`Core.Common.PaginationDefaults`), aplicada em todas as listagens.
- **Cache**: respostas de sucesso (200) das listagens e buscas por ID são gravadas na tabela `cache` já existente no banco, chaveadas por rota + querystring, com TTL padrão de 300s (`Cache:TtlSeconds`) — assunção própria, nenhum documento do desafio define TTL.

## Testes

```bash
dotnet test AuxiDesafio.slnx
```

Um projeto xUnit por API (`AutenticacaoApi.Tests`, `CondominiosApi.Tests`, `BlocosApi.Tests`, `UnidadesApi.Tests`), cobrindo handlers, paginação, emissão de JWT, login e o filtro de cache — com dublês para as dependências de banco (sem round-trip para o Supabase real).

## Decisões e limitações conhecidas

- **Bug corrigido**: `CondominioConfiguration` mapeava 7 colunas com nome divergente do schema real (ex. `cep8log` → `cep8_log`), o que quebrava qualquer consulta em `condominios`.
- Query params de Blocos e Unidades são decisão de projeto — nenhum dos dois é especificado no material do desafio.
- Login não implementa refresh token, bloqueio por tentativas falhas nem claims de papel/role, apesar de as tabelas (`usuario_sessoes`, `papeis`, `usuario_condominio_papeis`) existirem no banco — fora do escopo desta entrega.
- Os PDFs do desafio (Termo de Abertura, contrato de dados de Condomínio) estão em [`Docs/`](Docs/).
