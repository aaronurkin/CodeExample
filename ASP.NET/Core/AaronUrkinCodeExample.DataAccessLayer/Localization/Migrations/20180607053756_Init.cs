using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AaronUrkinCodeExample.DataAccessLayer.Localization.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "exmpl_localization");

            migrationBuilder.CreateTable(
                name: "Translations",
                schema: "exmpl_localization",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CultureCode = table.Column<string>(maxLength: 17, nullable: false),
                    Key = table.Column<string>(nullable: true),
                    Scope = table.Column<string>(maxLength: 512, nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Translations_CultureCode_Scope_Key",
                schema: "exmpl_localization",
                table: "Translations",
                columns: new[] { "CultureCode", "Scope", "Key" },
                unique: true,
                filter: "[Key] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Translations",
                schema: "exmpl_localization");
        }
    }
}
