using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shorty.Management.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyLinksAddStatusAndClicks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Links",
                type: "INTEGER",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "TotalClicks",
                table: "Links",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "TotalClicks",
                table: "Links");
        }
    }
}
