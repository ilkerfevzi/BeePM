using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeePM.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseline : Migration
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "Users", keyColumn: "Id", keyValue: 1);
            migrationBuilder.DeleteData(table: "Users", keyColumn: "Id", keyValue: 2);
            migrationBuilder.DeleteData(table: "Users", keyColumn: "Id", keyValue: 3);
            migrationBuilder.DeleteData(table: "Users", keyColumn: "Id", keyValue: 4);
        }
    }
}
