using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eladei.BookInfo.Model.Migrations;

public partial class BookInfoInitialMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "BookInformations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                Author = table.Column<string>(type: "text", nullable: false),
                Pages = table.Column<long>(type: "bigint", nullable: true),
                Circulation = table.Column<long>(type: "bigint", nullable: true),
                Annotation = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                Editor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Translator = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Artist = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BookInformations", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_BookInformations_Name_Author",
            table: "BookInformations",
            columns: new[] { "Name", "Author" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "BookInformations");
    }
}