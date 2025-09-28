using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BeePM.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
         table: "Users",
         columns: new[] { "Id", "Username", "FullName", "Role" },
         values: new object[,]
         {
            { 1, "employee1", "User 1 Çalışan", "Employee" },
            { 2, "manager1", "User 2 Müdür", "Manager" },
            { 3, "board1", "User 3 Yönetim Kurulu", "Board" },
            { 4, "admin", "Admin Kullanıcı", "Admin" }
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
