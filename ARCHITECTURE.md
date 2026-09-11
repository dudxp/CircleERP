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
| `CircleERP.Api.IntegrationTests` | a pilha completa por HTTP: rota, serializacao, handler, mapeamento e conversores |

Os testes de dominio rodam sem banco, sem HTTP e sem container -- se um deles
precisar de infraestrutura para rodar, a regra vazou de camada.

Os de integracao sobem a API em memoria com `WebApplicationFactory` e trocam o
MySQL por um SQLite em memoria, um por teste. Nao dependem de servidor externo
nem de Docker. A ressalva e que SQLite nao e MySQL: comportamento especifico do
provider (tipo de coluna, colacao) nao e verificado ali.

A fabrica de teste passa a string de conexao e a versao do servidor por
**variavel de ambiente**, e nao por `ConfigureAppConfiguration`. Com hosting
minimo, o que a fabrica registra em `ConfigureAppConfiguration` so e aplicado em
`Build()` -- depois de os top-level statements do `Program.cs` ja terem lido
`builder.Configuration`. Variavel de ambiente entra nas fontes padrao e chega a
tempo.

Para conferir que os testes nao dependem do ambiente da sua maquina:

```bash
env -u MYSQL_CONNECTION_STRING dotnet test CircleERP.sln
```

A regra de dependencia hoje e garantida estruturalmente pelos
`ProjectReference`. Se um dia for preciso verifica-la em teste (por exemplo,
proibir `using` de namespaces de infraestrutura na Application), o caminho e
adicionar `NetArchTest.Rules` ao projeto de testes.

## Agregados

### Currency

Moeda aceita pelo sistema. `code` e o codigo ISO 4217; `symbol` e opcional e
existe so para exibicao.

### Customer

Cliente. `Document` valida os digitos verificadores de CPF e CNPJ de verdade e
guarda apenas digitos -- sem isso, "529.982.247-25" e "52998224725" seriam dois
clientes. O indice unico no documento fecha a porta no banco.

O tipo (pessoa fisica ou juridica) **acompanha o documento** e nunca e informado
em paralelo: `SetDocument` escreve os dois juntos, entao nao existe caminho para
um cliente marcado como PJ portando um CPF.

Cliente nao se exclui, se inativa. Um cliente inativo nao recebe pedidos novos,
mas continua existindo -- os pedidos antigos dele precisam seguir legiveis.

### Address

Endereco. Raiz de agregado propria, com cadastro e tela proprios: o mesmo
endereco existe independente de haver cliente vinculado a ele.

`StateCode` recusa qualquer par de letras que nao seja uma das 27 UFs -- "XX"
tem o formato certo e mesmo assim nao e um estado. O numero e texto, porque
"S/N" e "123A" sao numeros de endereco validos.

O cliente referencia o endereco por identidade, sem chave estrangeira. Nada no
banco impede excluir um endereco em uso, entao a integridade e garantida no caso
de uso (`RemoveAddress` responde 409), e ha teste de integracao provando isso.

### Order

Pedido de venda. `OrderItem` e entidade interna: so existe atraves do pedido, e
por isso nao ha repositorio de itens -- a linha e sempre carregada e salva junto
da raiz.

O que o agregado garante:

| Invariante | Onde |
|---|---|
| itens so mudam enquanto o pedido esta em rascunho | `EnsureIsDraft` |
| quantidade maior que zero | `Quantity` |
| preco unitario nao negativo, com no maximo duas casas | `Money` |
| toda linha esta na moeda do pedido | `AddItem` constroi o `Money` a partir da moeda da raiz |
| pedido sem itens nao e confirmado | `Place` |
| pedido cancelado nao volta atras | `Cancel` |

O `Total` e uma propriedade calculada, nunca uma coluna: total gravado e total
que pode divergir das linhas.

O pedido guarda o **codigo** da moeda e o **id** do cliente, e nao referencias
aos agregados nem chaves estrangeiras. Agregados referenciam outros agregados por
identidade -- se o pedido carregasse a moeda inteira, um pedido antigo passaria
a valer pela taxa de hoje; se guardasse o nome do cliente, renomear o cliente
deixaria uma copia velha dentro do pedido.

O nome do cliente e resolvido na leitura, em uma consulta por listagem
(`GetByIdsAsync`), e nao uma por pedido. Um pedido cujo cliente sumiu do cadastro
vira "(cliente N)" em vez de quebrar a tela.

As regras "a moeda precisa estar cadastrada" e "o cliente precisa existir e estar
ativo" sao regras *entre* agregados, e por isso vivem no caso de uso.

Confirmar e cancelar sao acoes, nao alteracoes de campo. Na API aparecem como
`POST /api/orders/{id}/place` e `/cancel`, e nao como um `PATCH` em `status`:
assim nao existe requisicao capaz de pular uma etapa do ciclo.

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

## Frontend

O client segue a mesma ideia de camadas, organizado por feature em vez de por
tipo de arquivo:

```
src/
  app/         moldura da aplicacao, rotas, navegacao
  shared/      cliente HTTP, tipos e componentes reutilizaveis
  features/
    currency/
      api/     unico lugar que conhece as rotas de moeda
      model/   tipos da feature
      hooks/   estado de servidor (useCurrencies)
      ui/      componentes
      index.ts entrada publica
    customers/
    addresses/ expoe tambem o AddressDialog, usado de dentro da tela de cliente
    orders/
    home/      visao geral; consome as outras features pelo index.ts delas
```

Os campos do formulario de endereco vivem em um componente proprio
(`AddressFormFields`) porque dois lugares cadastram endereco: a aba propria e o
popup dentro da tela de cliente. Uma copia em cada lugar viraria duas telas
divergindo com o tempo.

A regra pratica: **componente nao conhece axios, hook nao conhece MUI, funcao
pura nao conhece nenhum dos dois.**

Uma feature importa livremente de `shared/` e de si mesma. Quando precisa de
outra feature -- pedidos precisa da lista de moedas para o seletor -- importa
apenas do `index.ts` dela, nunca de um caminho interno. Assim o que fica exposto
e uma decisao explicita, e reorganizar as pastas de dentro nao quebra ninguem.

`features/<nome>/api` traduz qualquer falha em `ApiError`, entao nenhum
componente inspeciona status HTTP. E o hook e a fonte unica da lista: as
operacoes de escrita recarregam do servidor em vez de reproduzir localmente o
que o backend fez, o que evita a tela divergir do banco.

Nao ha biblioteca de estado de servidor -- `useCurrencies` e escrito a mao. Se
um dia o cache entre telas comecar a doer, o substituto natural e o TanStack
Query, e o ponto de troca e o hook, nao os componentes.
