using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shorty.Management.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyOutboxAddError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Outbox",
                newName: "LastError");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastError",
                table: "Outbox",
                newName: "SentAt");
        }
    }
}
