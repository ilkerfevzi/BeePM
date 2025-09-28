using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeePM.Migrations
{
    /// <inheritdoc />
    public partial class SyncApprovalEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "FieldDefinitions");

            migrationBuilder.AddColumn<int>(
                name: "CreatedUserId",
                table: "ApprovalRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ApprovalRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestedItem",
                table: "ApprovalRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_CreatedUserId",
                table: "ApprovalRequests",
                column: "CreatedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalRequests_Users_CreatedUserId",
                table: "ApprovalRequests",
                column: "CreatedUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalRequests_Users_CreatedUserId",
                table: "ApprovalRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalRequests_CreatedUserId",
                table: "ApprovalRequests");

            migrationBuilder.DropColumn(
                name: "CreatedUserId",
                table: "ApprovalRequests");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ApprovalRequests");

            migrationBuilder.DropColumn(
                name: "RequestedItem",
                table: "ApprovalRequests");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FieldDefinitions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "FieldDefinitions",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Reason");

            migrationBuilder.UpdateData(
                table: "FieldDefinitions",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Item");

            migrationBuilder.UpdateData(
                table: "FieldDefinitions",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Quantity");
        }
    }
}
