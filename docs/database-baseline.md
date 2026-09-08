# Adotando migrations no banco existente

O schema do CircleERP foi criado manualmente. A partir da Fase 2 o EF Core
gerencia o schema, e a migration `20260908162539_InitialCreate` descreve o
**estado alvo** da tabela `currency`.

Como a tabela ja existe com dados, **nao rode `dotnet ef database update`**:
ele tentaria criar uma tabela que ja esta la. O caminho e fazer um *baseline* --
alinhar as colunas e registrar a migration como aplicada.

> Este documento foi escrito a partir do schema e dos dados reais do banco
> `CircleERP` (MySQL 5.7.40), lidos em 2026-09-08.

## Schema atual x alvo

```sql
-- hoje
CREATE TABLE `currency` (
  `id`          int(11)    NOT NULL AUTO_INCREMENT,
  `code`        varchar(3) NOT NULL DEFAULT '',
  `description` text       NOT NULL,
  `rating`      float      NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1
```

| Coluna | Hoje | Alvo | Por que |
|---|---|---|---|
| `rating` | `float` | `decimal(18,6)` | ver abaixo |
| `code` | `varchar(3)`, sem indice | `varchar(3)`, indice unico | o codigo e a chave natural; a unicidade passa a ser do banco, e nao so da verificacao previa do handler, que sofre corrida |
| `description` | `text NOT NULL` | `varchar(100) NOT NULL` | a maior descricao atual tem 9 caracteres |
| `symbol` | nao existe | `varchar(5) NULL` | separa o simbolo de exibicao ("R$") do codigo ISO ("BRL") |
| charset | `latin1` | `utf8mb4` | os dados atuais sao todos ASCII, entao a conversao e byte a byte identica |

### Sobre `float` -> `decimal`

Nao e uma troca cosmetica. O valor que aparece como `5.92` na aplicacao esta
gravado como:

```
id=1  code=R$  rating=5.920000076293945
```

Isso e o erro de representacao do ponto flutuante binario. Converter para
`decimal(18,6)` grava `5.920000` -- ou seja, a conversao **corrige** o valor em
vez de estraga-lo. O mesmo vale para `0.4` (`0.4000000059604645`) e `5.43`
(`5.429999828338623`).

## Passo 1 -- conferir

```sql
SHOW CREATE TABLE `currency`;

-- Codigos duplicados impediriam o indice unico (hoje: nenhum):
SELECT UPPER(TRIM(code)) AS code, COUNT(*) AS total
FROM `currency` GROUP BY 1 HAVING total > 1;

-- Linhas fora do formato ISO 4217 (hoje: ids 1, 5, 12, 13, 14, 15):
SELECT id, code, description FROM `currency`
WHERE CHAR_LENGTH(TRIM(code)) <> 3 OR TRIM(code) REGEXP '[^A-Za-z]';
```

## Passo 2 -- criar a coluna `symbol`

```sql
ALTER TABLE `currency`
  ADD COLUMN `symbol` varchar(5) CHARACTER SET utf8mb4 NULL AFTER `rating`;
```

## Passo 3 -- separar simbolo de codigo

`R$` e `US$` sao simbolos, nao codigos ISO. Vao para a coluna nova, e o `code`
recebe o codigo correspondente:

```sql
UPDATE `currency` SET `code` = 'BRL', `symbol` = 'R$'  WHERE `id` = 1;   -- Real
UPDATE `currency` SET `code` = 'USD', `symbol` = 'US$' WHERE `id` = 5;   -- Dolar

-- Opcional, so para completar o cadastro:
UPDATE `currency` SET `symbol` = 'JPY' WHERE `id` = 2;                   -- iene
```

## Passo 4 -- resolver as linhas de teste

Os ids 12 a 15 (`TT`/"TESTESSSS", `FF`/"FEEE", `VV`/"VERR", `ge`/"aa") tem
codigo de 2 letras e nao passam na validacao ISO. **Escolha uma das duas
opcoes.** Os ids 4 (`TES`) e 16 (`TRE`) tem 3 letras e passam, entao ficam como
estao -- apague tambem se forem lixo.

Apagar:

```sql
-- Confira antes: SELECT * FROM `currency` WHERE `id` IN (12,13,14,15);
DELETE FROM `currency` WHERE `id` IN (12, 13, 14, 15);
```

Ou manter, completando os codigos para tres letras:

```sql
UPDATE `currency` SET `code` = 'TTT' WHERE `id` = 12;
UPDATE `currency` SET `code` = 'FFF' WHERE `id` = 13;
UPDATE `currency` SET `code` = 'VVV' WHERE `id` = 14;
UPDATE `currency` SET `code` = 'GEE' WHERE `id` = 15;
```

> Enquanto existir uma linha com codigo fora do formato, `GET /api/currencies`
> falha: o conversor do value object valida cada registro na leitura.

## Passo 5 -- alinhar os tipos

```sql
ALTER TABLE `currency`
  MODIFY COLUMN `code`        varchar(3)    CHARACTER SET utf8mb4 NOT NULL,
  MODIFY COLUMN `description` varchar(100)  CHARACTER SET utf8mb4 NOT NULL,
  MODIFY COLUMN `rating`      decimal(18,6) NOT NULL,
  MODIFY COLUMN `symbol`      varchar(5)    CHARACTER SET utf8mb4 NULL;

ALTER TABLE `currency` DEFAULT CHARACTER SET utf8mb4;

CREATE UNIQUE INDEX `IX_currency_code` ON `currency` (`code`);
```

Os `DEFAULT ''` de `code` e `DEFAULT 0` de `rating` somem junto -- valor padrao
no banco mascara o esquecimento de preencher o campo.

## Passo 6 -- registrar a migration como aplicada

```sql
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId`    varchar(150) NOT NULL,
    `ProductVersion` varchar(32)  NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260908162539_InitialCreate', '9.0.11');
```

## Passo 7 -- conferir

```bash
dotnet tool restore
dotnet ef migrations list --project CircleERP.Infrastructure --startup-project CircleERP.Infrastructure
```

`InitialCreate` deve aparecer como aplicada. Dali em diante o fluxo e o normal:

```bash
dotnet ef database update --project CircleERP.Infrastructure --startup-project CircleERP.Infrastructure
```

## Banco novo (sem dados)

Nada disso e necessario -- so:

```bash
dotnet ef database update --project CircleERP.Infrastructure --startup-project CircleERP.Infrastructure
```

## Por que `--startup-project` aponta para a propria Infrastructure

`CircleERP.Infrastructure` tem um `IDesignTimeDbContextFactory` que fixa a
versao do servidor (5.7.40), entao gerar e aplicar migrations nao depende da
configuracao do host. A string de conexao vem da variavel de ambiente
`MYSQL_CONNECTION_STRING`.
