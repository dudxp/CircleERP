# Arquitetura

O CircleERP segue Clean Architecture com modelagem de dominio (DDD). O objetivo
e que a regra de negocio seja testavel sem banco, sem HTTP e sem framework.

## Camadas

```
┌──────────────────────────────────────────────────────────┐
│  CircleERP (host / API)          composition root        │
│  Controllers, DI, middleware, CORS, OpenAPI              │
└───────────────┬──────────────────────────┬───────────────┘
                │                          │
                ▼                          ▼
┌───────────────────────────┐  ┌───────────────────────────┐
│  CircleERP.Application    │  │  CircleERP.Infrastructure │
│  casos de uso (MediatR)   │◄─┤  EF Core, MySQL,          │
│  contratos de porta       │  │  repositorios, UnitOfWork │
└───────────────┬───────────┘  └───────────────────────────┘
                │
                ▼
┌──────────────────────────────────────────────────────────┐
│  CircleERP.Domain                                        │
│  agregados, entidades, value objects, eventos            │
│  ZERO dependencias                                       │
└──────────────────────────────────────────────────────────┘
```

## A regra de dependencia

As setas so apontam para dentro. Na pratica isso e garantido pelos
`ProjectReference` de cada `.csproj` -- nao existe caminho de compilacao que
permita ao dominio enxergar EF Core:

| Projeto | Pode referenciar |
|---|---|
| `CircleERP.Domain` | **nada** (nem projeto, nem pacote) |
| `CircleERP.Application` | `Domain` |
| `CircleERP.Infrastructure` | `Application`, `Domain` |
| `CircleERP` (host) | `Application`, `Infrastructure` |

`Infrastructure` so e referenciada pelo host, e apenas para registrar as
implementacoes no container. Nenhum controller conhece `DbContext`.

## Building blocks do dominio

| Tipo | Quando usar |
|---|---|
| `Entity<TId>` | tem identidade propria e ciclo de vida; comparada por `Id` |
| `ValueObject` | definido pelos atributos, imutavel, valida no construtor |
| `IAggregateRoot` | unica entidade do agregado referenciavel de fora |
| `IDomainEvent` | fato de negocio que outras partes podem querer observar |
| `DomainException` | invariante violada (diferente de erro de aplicacao) |

Regra pratica: **se um objeto pode existir em estado invalido, a modelagem esta
errada.** Validacao de formato e faixa vive no construtor do value object, nao
em `if`s espalhados por services.

## Onde ficam os contratos

As interfaces de repositorio (`ICurrencyRepository`) ficam no **dominio**, ao
lado do agregado que elas carregam: quem define o que precisa saber sobre uma
moeda e o dominio, e a infraestrutura apenas atende. Ja o `IUnitOfWork` fica na
**Application**, porque transacao e uma preocupacao de orquestracao do caso de
uso, nao uma regra de negocio.

O `AppDbContext` implementa `IUnitOfWork`. Para as camadas de dentro, ele e
apenas "algo que confirma alteracoes".

## Casos de uso

Cada caso de uso e um `ICommand`/`IQuery` com um unico handler, despachado por
MediatR. Comandos alteram estado e devolvem `Result`; consultas apenas leem.
O handler orquestra: carrega o agregado pelo repositorio, chama um metodo de
negocio nele, e confirma via `IUnitOfWork`. Ele nao contem regra de negocio.

## Testes

| Projeto | Cobre |
|---|---|
| `CircleERP.Domain.Tests` | invariantes dos agregados e dos building blocks; sem I/O |

Os testes rodam sem banco, sem HTTP e sem container -- se um teste de dominio
precisar de infraestrutura para rodar, a regra vazou de camada.

A regra de dependencia hoje e garantida estruturalmente pelos
`ProjectReference`. Se um dia for preciso verifica-la em teste (por exemplo,
proibir `using` de namespaces de infraestrutura na Application), o caminho e
adicionar `NetArchTest.Rules` ao projeto de testes.

## Erros

Sao duas coisas diferentes, e a borda trata cada uma de um jeito:

| Situacao | Como o dominio/aplicacao expressa | HTTP |
|---|---|---|
| Entrada viola uma invariante (codigo com 4 letras, taxa negativa) | `DomainException`, lancada pelo value object | 400 |
| Recurso nao existe | `Result.Fail(NotFoundError)` | 404 |
| Conflito com o estado atual (codigo ja cadastrado) | `Result.Fail(ConflictError)` | 409 |

A traducao acontece em dois lugares unicos: `DomainExceptionHandler` para o
primeiro caso e `ResultExtensions` para os demais. Nenhuma action repete essa
decisao, e todas respondem em `ProblemDetails`.

## Banco de dados

O schema e gerenciado por migrations do EF Core, em
`CircleERP.Infrastructure/Persistence/Migrations`. O banco que ja existia foi
adotado pelas proprias migrations, sem passo manual: a `InitialCreate` usa
`CREATE TABLE IF NOT EXISTS` e e no-op num banco que ja tem a tabela. Ver
[docs/database-baseline.md](docs/database-baseline.md).

O mapeamento fica em `IEntityTypeConfiguration`, nunca em atributos na entidade:
o dominio nao carrega anotacao de persistencia. Value objects de um unico campo
sao mapeados com `HasConversion`, entao `code` continua sendo uma coluna
`varchar` comum no banco.

O conversor roda **na leitura tambem**: um registro que nao passe na validacao
do value object faz a consulta falhar. Isso e intencional -- e melhor descobrir
que existe dado invalido do que carrega-lo para dentro do dominio -- mas
significa que limpar os dados faz parte de introduzir uma regra nova.

Nomes de tabela e coluna sao minusculos, como no banco. Nesta maquina o MySQL
roda com `lower_case_table_names=1` e a comparacao e insensivel, mas em Linux o
padrao e `0` e `CURRENCY` nao encontraria `currency`.
