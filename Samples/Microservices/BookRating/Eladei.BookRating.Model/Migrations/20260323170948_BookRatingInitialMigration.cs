using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eladei.BookRating.Model.Migrations;

public partial class BookRatingInitialMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Books",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                Author = table.Column<string>(type: "text", nullable: false),
                Votes = table.Column<long>(type: "bigint", nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Books", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "IntegrationEvents",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                EventType = table.Column<string>(type: "text", nullable: false),
                EventMetadata = table.Column<string>(type: "text", nullable: false),
                IsSent = table.Column<bool>(type: "boolean", nullable: false),
                NumberOfSendingAttempts = table.Column<int>(type: "integer", nullable: false),
                SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                LastError = table.Column<string>(type: "text", nullable: true),
                ReservedBy = table.Column<Guid>(type: "uuid", nullable: true),
                ReservedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_IntegrationEvents", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Books_Name_Author",
            table: "Books",
            columns: new[] { "Name", "Author" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Books");

        migrationBuilder.DropTable(
            name: "IntegrationEvents");
    }
}