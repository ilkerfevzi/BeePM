using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BeePM.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FieldDefinitions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FieldDefinitions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FieldDefinitions",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "FullName", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "User 1 Çalışan", null, "Employee", "employee1" },
                    { 2, "User 2 Müdür", null, "Manager", "manager1" },
                    { 3, "User 3 Yönetim Kurulu", null, "Board", "board1" },
                    { 4, "Admin Kullanıcı", null, "Admin", "admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.InsertData(
                table: "FieldDefinitions",
                columns: new[] { "Id", "FieldType", "Label", "Options" },
                values: new object[,]
                {
                    { 1, "Textbox", "Talep Nedeni", null },
                    { 2, "Combobox", "Talep Edilen Ürün", "[\"Laptop\",\"Telefon\",\"Tablet\"]" },
                    { 3, "Numeric", "Adet", null }
                });
        }
    }
}
