using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeePM.Migrations
{
    /// <inheritdoc />
    public partial class AddFormTemplateToApprovalRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormDataJson",
                table: "ApprovalRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FormTemplateId",
                table: "ApprovalRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_FormTemplateId",
                table: "ApprovalRequests",
                column: "FormTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalRequests_FormTemplates_FormTemplateId",
                table: "ApprovalRequests",
                column: "FormTemplateId",
                principalTable: "FormTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalRequests_FormTemplates_FormTemplateId",
                table: "ApprovalRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApprovalRequests_FormTemplateId",
                table: "ApprovalRequests");

            migrationBuilder.DropColumn(
                name: "FormDataJson",
                table: "ApprovalRequests");

            migrationBuilder.DropColumn(
                name: "FormTemplateId",
                table: "ApprovalRequests");
        }
    }
}
