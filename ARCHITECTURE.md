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

## Casos de uso

Cada caso de uso e um `ICommand`/`IQuery` com um unico handler, despachado por
MediatR. Comandos alteram estado e devolvem `Result`; consultas apenas leem.
O handler orquestra: carrega o agregado pelo repositorio, chama um metodo de
negocio nele, e confirma via `IUnitOfWork`. Ele nao contem regra de negocio.

## Testes

| Projeto | Cobre |
|---|---|
| `CircleERP.Domain.Tests` | invariantes dos agregados e dos building blocks; sem I/O |

A regra de dependencia hoje e garantida estruturalmente pelos
`ProjectReference`. Se um dia for preciso verifica-la em teste (por exemplo,
proibir `using` de namespaces de infraestrutura na Application), o caminho e
adicionar `NetArchTest.Rules` ao projeto de testes.
