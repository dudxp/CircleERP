using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CircleERP.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Adota o schema que ja existia no banco, criado manualmente antes de o
    /// projeto usar migrations.
    /// </summary>
    /// <remarks>
    /// O corpo e escrito a mao, e nao gerado pelo diff do modelo, por causa do
    /// <c>IF NOT EXISTS</c>: num banco novo a tabela e criada; no banco que ja
    /// existe esta migration nao faz nada. Isso permite adotar o schema legado
    /// rodando apenas <c>dotnet ef database update</c>, sem inserir a mao a
    /// linha correspondente em <c>__EFMigrationsHistory</c>.
    ///
    /// A tabela e criada aqui no formato ANTIGO (rating float, description text,
    /// sem symbol, sem indice unico). Quem leva o schema ao formato do modelo e
    /// a migration seguinte, para que banco novo e banco existente passem
    /// exatamente pelo mesmo caminho.
    /// </remarks>
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE TABLE IF NOT EXISTS `currency` (
                  `id`          int(11)    NOT NULL AUTO_INCREMENT,
                  `code`        varchar(3) NOT NULL DEFAULT '',
                  `description` text       NOT NULL,
                  `rating`      float      NOT NULL DEFAULT '0',
                  PRIMARY KEY (`id`)
                ) ENGINE=InnoDB DEFAULT CHARSET=latin1;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS `currency`;");
        }
    }
}
