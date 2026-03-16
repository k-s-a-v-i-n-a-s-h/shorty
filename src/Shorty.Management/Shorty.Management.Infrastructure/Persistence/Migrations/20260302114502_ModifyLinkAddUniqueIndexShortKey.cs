using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shorty.Management.Domain.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyLinkAddUniqueIndexShortKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Links_ShortKey",
                table: "Links");

            migrationBuilder.CreateIndex(
                name: "IX_Links_ShortKey",
                table: "Links",
                column: "ShortKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Links_ShortKey",
                table: "Links");

            migrationBuilder.CreateIndex(
                name: "IX_Links_ShortKey",
                table: "Links",
                column: "ShortKey");
        }
    }
}
