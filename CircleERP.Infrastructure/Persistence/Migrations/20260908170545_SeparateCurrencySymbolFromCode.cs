using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CircleERP.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Leva a tabela `currency` do formato legado ao formato do modelo:
    /// separa o simbolo de exibicao do codigo ISO, troca float por decimal,
    /// converte o charset e garante a unicidade do codigo.
    /// </summary>
    public partial class SeparateCurrencySymbolFromCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. A coluna nova entra aceitando nulo: ausencia de simbolo e um
            //    estado valido, e as linhas existentes ainda nao tem simbolo.
            migrationBuilder.Sql(
                """
                ALTER TABLE `currency`
                  ADD COLUMN `symbol` varchar(5) CHARACTER SET utf8mb4 NULL AFTER `rating`;
                """);

            // 2. "R$" e "US$" estavam gravados como codigo, mas sao simbolos.
            //    O filtro e pelo proprio valor, e nao por id, para que a
            //    migration funcione em qualquer banco -- inclusive num vazio,
            //    onde simplesmente nao afeta nenhuma linha.
            migrationBuilder.Sql(
                """
                UPDATE `currency` SET `code` = 'BRL', `symbol` = 'R$'  WHERE `code` = 'R$';
                """);
            migrationBuilder.Sql(
                """
                UPDATE `currency` SET `code` = 'USD', `symbol` = 'US$' WHERE `code` = 'US$';
                """);

            // 3. float -> decimal. O valor exibido como 5.92 estava gravado como
            //    5.920000076293945; a conversao corrige a representacao.
            //    text -> varchar(100) e latin1 -> utf8mb4 no mesmo passo.
            migrationBuilder.Sql(
                """
                ALTER TABLE `currency`
                  MODIFY COLUMN `code`        varchar(3)    CHARACTER SET utf8mb4 NOT NULL,
                  MODIFY COLUMN `description` varchar(100)  CHARACTER SET utf8mb4 NOT NULL,
                  MODIFY COLUMN `rating`      decimal(18,6) NOT NULL,
                  MODIFY COLUMN `symbol`      varchar(5)    CHARACTER SET utf8mb4 NULL;
                """);

            // Os DEFAULT '' e DEFAULT 0 saem junto: valor padrao no banco
            // mascara o esquecimento de preencher o campo.
            migrationBuilder.Sql("ALTER TABLE `currency` DEFAULT CHARACTER SET utf8mb4;");

            // 4. O codigo e a chave natural. Ate aqui a unicidade dependia so da
            //    verificacao previa do handler, que sofre corrida.
            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX `IX_currency_code` ON `currency` (`code`);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX `IX_currency_code` ON `currency`;");

            migrationBuilder.Sql(
                """
                UPDATE `currency` SET `code` = `symbol` WHERE `symbol` IS NOT NULL;
                """);

            migrationBuilder.Sql(
                """
                ALTER TABLE `currency`
                  MODIFY COLUMN `code`        varchar(3) CHARACTER SET latin1 NOT NULL DEFAULT '',
                  MODIFY COLUMN `description` text       CHARACTER SET latin1 NOT NULL,
                  MODIFY COLUMN `rating`      float      NOT NULL DEFAULT 0;
                """);

            migrationBuilder.Sql("ALTER TABLE `currency` DROP COLUMN `symbol`;");
            migrationBuilder.Sql("ALTER TABLE `currency` DEFAULT CHARACTER SET latin1;");
        }
    }
}
