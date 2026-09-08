# Banco de dados

O schema do CircleERP foi criado manualmente, antes de o projeto usar
migrations. A adocao do EF Core foi feita de forma que **nao existe passo
manual**: banco novo e banco existente passam pelo mesmo comando.

```bash
dotnet tool restore
dotnet ef database update --project CircleERP.Infrastructure --startup-project CircleERP.Infrastructure
```

> Este documento parte do schema e dos dados reais do banco `CircleERP`
> (MySQL 5.7.40), lidos em 2026-09-08.

## Como as duas migrations se encaixam

| Migration | O que faz |
|---|---|
| `20260908162539_InitialCreate` | `CREATE TABLE IF NOT EXISTS currency` no **formato antigo** (rating `float`, description `text`, sem `symbol`, sem indice unico) |
| `20260908170545_SeparateCurrencySymbolFromCode` | leva a tabela ao formato do modelo |

A primeira e escrita a mao em vez de gerada pelo diff, por causa do
`IF NOT EXISTS`: num banco novo ela cria a tabela; no banco que ja existe ela
nao faz nada, e o EF registra a migration como aplicada do mesmo jeito. E isso
que dispensa o `INSERT` manual em `__EFMigrationsHistory` que a adocao de
migrations normalmente exige.

Como a tabela e criada no formato antigo, **os dois cenarios percorrem
exatamente o mesmo caminho** -- o que a segunda migration faz no seu banco e o
mesmo que ela faz num banco recem-criado.

## O que a segunda migration muda

| Coluna | Antes | Depois | Por que |
|---|---|---|---|
| `rating` | `float` | `decimal(18,6)` | ver abaixo |
| `code` | `varchar(3)`, sem indice | `varchar(3)`, indice unico | o codigo e a chave natural; a unicidade passa a ser do banco, e nao so da verificacao previa do handler, que sofre corrida |
| `description` | `text NOT NULL` | `varchar(100) NOT NULL` | a maior descricao atual tem 9 caracteres |
| `symbol` | nao existia | `varchar(5) NULL` | separa o simbolo de exibicao ("R$") do codigo ISO ("BRL") |
| charset | `latin1` | `utf8mb4` | os dados atuais sao todos ASCII, entao a conversao e byte a byte identica |

Alem disso, migra os dados que confundiam simbolo com codigo:

```sql
UPDATE `currency` SET `code` = 'BRL', `symbol` = 'R$'  WHERE `code` = 'R$';
UPDATE `currency` SET `code` = 'USD', `symbol` = 'US$' WHERE `code` = 'US$';
```

O filtro e pelo valor, nao por id, para que a migration funcione em qualquer
banco -- num banco vazio ela simplesmente nao afeta nenhuma linha.

### Sobre `float` -> `decimal`

Nao e uma troca cosmetica. O valor que a aplicacao exibe como `5.92` esta
gravado como:

```
id=1  code=R$  rating=5.920000076293945
```

E o erro de representacao do ponto flutuante binario. Converter para
`decimal(18,6)` grava `5.920000` -- a conversao **corrige** o valor em vez de
estraga-lo. Mesmo caso em `0.4` (`0.4000000059604645`) e `5.43`
(`5.429999828338623`).

## Pendencia: linhas de teste

Os ids 12 a 15 tem codigo de duas letras e **nao** sao tratados pela migration:

| id | code | description |
|---|---|---|
| 12 | `TT` | TESTESSSS |
| 13 | `FF` | FEEE |
| 14 | `VV` | VERR |
| 15 | `ge` | aa |

Enquanto existirem, `GET /api/currencies` falha: o conversor do value object
valida cada registro **na leitura**, e um codigo de duas letras nao passa na
regra ISO 4217. Escolha um dos dois caminhos e execute:

```sql
-- Apagar (confira antes: SELECT * FROM `currency` WHERE `id` IN (12,13,14,15);)
DELETE FROM `currency` WHERE `id` IN (12, 13, 14, 15);
```

```sql
-- Ou manter, completando os codigos para tres letras
UPDATE `currency` SET `code` = 'TTT' WHERE `id` = 12;
UPDATE `currency` SET `code` = 'FFF' WHERE `id` = 13;
UPDATE `currency` SET `code` = 'VVV' WHERE `id` = 14;
UPDATE `currency` SET `code` = 'GEE' WHERE `id` = 15;
```

Os ids 4 (`TES`) e 16 (`TRE`) tem tres letras e passam na validacao, entao a
aplicacao os carrega normalmente -- apague tambem se forem lixo.

## Conferindo

```bash
dotnet ef migrations list --project CircleERP.Infrastructure --startup-project CircleERP.Infrastructure
```

Para ver o SQL sem executar nada:

```bash
dotnet ef migrations script --idempotent --project CircleERP.Infrastructure --startup-project CircleERP.Infrastructure
```

## Por que `--startup-project` aponta para a propria Infrastructure

`CircleERP.Infrastructure` tem um `IDesignTimeDbContextFactory` que fixa a
versao do servidor (5.7.40), entao gerar e aplicar migrations nao depende da
configuracao do host. A string de conexao vem da variavel de ambiente
`MYSQL_CONNECTION_STRING`.
