using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prog7311_Part2.Migrations
{
    /// <inheritdoc />
    public partial class SuppressWarningsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$DWDUhgCrjzXpiS2BcvgCxeJdN9wVwiEevz7py7GizSRVTl9TkthjG");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$WeWspXsa.rheeHuBmFNefeNphhbe5m2NYiY0oNOF7ixH49ToyfTNC");
        }
    }
}
