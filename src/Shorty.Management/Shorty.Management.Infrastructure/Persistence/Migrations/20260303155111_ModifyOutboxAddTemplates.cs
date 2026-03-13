using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shorty.Management.Domain.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyOutboxAddTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Payload",
                table: "Outbox",
                newName: "Subject");

            migrationBuilder.AddColumn<string>(
                name: "HtmlBody",
                table: "Outbox",
                type: "TEXT",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Recipient",
                table: "Outbox",
                type: "TEXT",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    HtmlBody = table.Column<string>(type: "TEXT", nullable: false),
                    Subject = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.Type);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Outbox_Type",
                table: "Outbox",
                column: "Type");

            migrationBuilder.AddForeignKey(
                name: "FK_Outbox_Templates_Type",
                table: "Outbox",
                column: "Type",
                principalTable: "Templates",
                principalColumn: "Type",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Outbox_Templates_Type",
                table: "Outbox");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropIndex(
                name: "IX_Outbox_Type",
                table: "Outbox");

            migrationBuilder.DropColumn(
                name: "HtmlBody",
                table: "Outbox");

            migrationBuilder.DropColumn(
                name: "Recipient",
                table: "Outbox");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Outbox",
                newName: "Payload");
        }
    }
}
