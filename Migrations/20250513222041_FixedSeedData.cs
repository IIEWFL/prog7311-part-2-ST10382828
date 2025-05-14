using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prog7311_Part2.Migrations
{
    /// <inheritdoc />
    public partial class FixedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$WeWspXsa.rheeHuBmFNefeNphhbe5m2NYiY0oNOF7ixH49ToyfTNC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 5, 14, 0, 8, 27, 421, DateTimeKind.Local).AddTicks(9784), "$2a$11$Js9H.7N32iQoK8kGIkMakehB3327o6J3KAD5gcxVQvJaxgu74UQ/2" });
        }
    }
}
