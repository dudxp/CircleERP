# Adotando migrations num banco que ja existe

O schema do CircleERP foi criado manualmente, sem migrations. A partir da
Fase 2 o EF Core passa a gerenciar o schema, e a migration inicial
(`20260908160941_InitialCreate`) descreve o **estado alvo** da tabela
`CURRENCY`.

Como a tabela ja existe, **nao rode `dotnet ef database update` direto**: ele
tentaria criar uma tabela que ja esta la e falharia. O caminho e fazer um
*baseline*: registrar a migration inicial como aplicada e ajustar as colunas
que mudaram.

## O que mudou em relacao ao schema antigo

| Coluna | Antes | Agora | Por que |
|---|---|---|---|
| `RATING` | `float` | `decimal(18,6)` | ponto flutuante binario nao representa `0.1` exatamente; o erro se acumula a cada conversao |
| `CODE` | sem limite/indice | `varchar(3)`, indice unico | o codigo e a chave natural; a unicidade passa a ser garantida pelo banco, nao so pela verificacao previa do handler (que sofre corrida) |
| `DESCRIPTION` | sem limite, aceitava nulo | `varchar(100)` `NOT NULL` | a descricao e obrigatoria no dominio |

## Passo 1 -- conferir o que esta no banco hoje

Antes de qualquer `ALTER`, veja o schema real e procure dados que impediriam a
mudanca:

```sql
SHOW CREATE TABLE `CURRENCY`;

-- Codigos duplicados impedem o indice unico:
SELECT UPPER(TRIM(CODE)) AS code, COUNT(*) AS total
FROM `CURRENCY` GROUP BY 1 HAVING total > 1;

-- Linhas que nao cabem no novo formato:
SELECT * FROM `CURRENCY`
WHERE CHAR_LENGTH(TRIM(CODE)) <> 3
   OR DESCRIPTION IS NULL
   OR CHAR_LENGTH(DESCRIPTION) > 100;
```

Se alguma dessas consultas devolver linhas, corrija os dados primeiro.

## Passo 2 -- alinhar as colunas

```sql
UPDATE `CURRENCY` SET CODE = UPPER(TRIM(CODE));

ALTER TABLE `CURRENCY`
  MODIFY COLUMN `CODE`        varchar(3)     CHARACTER SET utf8mb4 NOT NULL,
  MODIFY COLUMN `DESCRIPTION` varchar(100)   CHARACTER SET utf8mb4 NOT NULL,
  MODIFY COLUMN `RATING`      decimal(18,6)  NOT NULL;

CREATE UNIQUE INDEX `IX_CURRENCY_CODE` ON `CURRENCY` (`CODE`);
```

> A conversao de `float` para `decimal` arredonda para 6 casas. Se a taxa tinha
> mais precisao que isso, o valor muda -- confira antes se isso importa para voce.

## Passo 3 -- registrar a migration como aplicada

```sql
CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
    `MigrationId`    varchar(150) NOT NULL,
    `ProductVersion` varchar(32)  NOT NULL,
    CONSTRAINT `PK___EFMigrationsHistory` PRIMARY KEY (`MigrationId`)
) CHARACTER SET=utf8mb4;

INSERT INTO `__EFMigrationsHistory` (`MigrationId`, `ProductVersion`)
VALUES ('20260908160941_InitialCreate', '9.0.11');
```

## Passo 4 -- conferir

```bash
dotnet ef migrations list --project CircleERP.Infrastructure --startup-project CircleERP.Infrastructure
```

A `InitialCreate` deve aparecer como aplicada. Da proxima migration em diante o
fluxo e o normal:

```bash
dotnet ef database update --project CircleERP.Infrastructure --startup-project CircleERP.Infrastructure
```

## Banco novo (sem dados)

Nada disso e necessario -- basta:

```bash
dotnet ef database update --project CircleERP.Infrastructure --startup-project CircleERP.Infrastructure
```

## Por que `--startup-project` aponta para a propria Infrastructure

`CircleERP.Infrastructure` tem um `IDesignTimeDbContextFactory` que fixa a
versao do servidor, entao gerar e aplicar migrations nao depende de um MySQL
no ar nem da configuracao do host. A string de conexao vem da variavel de
ambiente `MYSQL_CONNECTION_STRING`.
