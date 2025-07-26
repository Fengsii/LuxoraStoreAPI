using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LuxoraStore.Migrations
{
    /// <inheritdoc />
    public partial class b : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "CreatedAt", "Email", "Image", "Name", "Password", "PhoneNumber", "Role", "UpdatedAt", "UserStatus", "Username" },
                values: new object[] { 1, "Bandung", new DateTime(2025, 7, 26, 12, 18, 58, 921, DateTimeKind.Utc).AddTicks(3991), "admin@example.com", "default-product.png.jpeg", "Administrator", "$2b$10$MjACVNV4YNUGhs3TphXVnuisiuxr1KkyEG2Cfs/iQlCmURb5N7kTK", "081267874199", "Admin", new DateTime(2025, 7, 26, 12, 18, 58, 673, DateTimeKind.Utc).AddTicks(7877), 0, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
