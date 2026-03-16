using Microsoft.EntityFrameworkCore.Migrations;
using Shorty.Management.Domain.Enums;

#nullable disable

namespace Shorty.Management.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InsertVerifyEmailTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Templates",
                columns: ["Type", "Subject", "HtmlBody", "CreatedAt"],
                values:
                [
                    TemplateType.VerifyEmail.ToString(),
                    "Verify your email address",
                    """<!DOCTYPE html><html><body style="font-family: 'Roboto', -apple-system, BlinkMacSystemFont, 'Segoe UI', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 1.5; color: #000; padding: 20px;"><p>Hi there,</p><p> Please click the link below to confirm your email address and activate your account: </p><p><a href="{{VerificationUrl}}" style="color: #007bff; text-decoration: underline; word-break: break-all;">{{VerificationUrl}}</a></p><p style="font-size: 14px; color: #555; margin-top: 30px;"><i>Note: If you did not request this, please disregard this message.</i></p><p style="margin-top: 25px; color: #444;"><i>Team Shorty</i></p></body></html>""",
                    DateTime.UtcNow,
                ]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Templates",
                keyColumn: "Type",
                keyValue: TemplateType.VerifyEmail.ToString());
        }
    }
}
