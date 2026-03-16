using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shorty.Management.Domain.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyLinksSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Links_ShortKey",
                table: "Links");

            migrationBuilder.RenameColumn(
                name: "ShortKey",
                table: "Links",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "DestinationUrl",
                table: "Links",
                newName: "Slug");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiresAt",
                table: "Links",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Links_ExpiresAt",
                table: "Links",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_Links_Slug",
                table: "Links",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Links_ExpiresAt",
                table: "Links");

            migrationBuilder.DropIndex(
                name: "IX_Links_Slug",
                table: "Links");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Links",
                newName: "ShortKey");

            migrationBuilder.RenameColumn(
                name: "Slug",
                table: "Links",
                newName: "DestinationUrl");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiresAt",
                table: "Links",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_Links_ShortKey",
                table: "Links",
                column: "ShortKey",
                unique: true);
        }
    }
}
