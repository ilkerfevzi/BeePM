using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeePM.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalFlows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StepNumber = table.Column<int>(type: "int", nullable: false),
                    ApproverRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalFlows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FieldDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Options = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedItem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_Users_CreatedUserId",
                        column: x => x.CreatedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRequestFields",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRequestFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalRequestFields_ApprovalRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ApprovalRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalRequestFields_FieldDefinitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "FieldDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalLogs_UserId",
                table: "ApprovalLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequestFields_FieldDefinitionId",
                table: "ApprovalRequestFields",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequestFields_RequestId",
                table: "ApprovalRequestFields",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_CreatedUserId",
                table: "ApprovalRequests",
                column: "CreatedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalFlows");

            migrationBuilder.DropTable(
                name: "ApprovalLogs");

            migrationBuilder.DropTable(
                name: "ApprovalRequestFields");

            migrationBuilder.DropTable(
                name: "ApprovalRequests");

            migrationBuilder.DropTable(
                name: "FieldDefinitions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
