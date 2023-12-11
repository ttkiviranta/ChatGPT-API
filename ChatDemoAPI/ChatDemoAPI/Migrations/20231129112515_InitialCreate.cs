using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatDemoAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatRobots",
                columns: table => new
                {
                    ChatRobotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatRobotName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ChatRobotDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRobots", x => x.ChatRobotId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserDescription = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "ChatRobotsDescription",
                columns: table => new
                {
                    ChatRobotDescriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatRobotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRobotsDescription", x => x.ChatRobotDescriptionId);
                    table.ForeignKey(
                        name: "FK_ChatRobotDescription_ChatRobot",
                        column: x => x.ChatRobotId,
                        principalTable: "ChatRobots",
                        principalColumn: "ChatRobotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatRobotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_Document_ChatRobot",
                        column: x => x.ChatRobotId,
                        principalTable: "ChatRobots",
                        principalColumn: "ChatRobotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChatRobotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChatHistoryContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReferencedDocumentDetailsIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatHistory_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentDetail",
                columns: table => new
                {
                    DocumentDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentSequence = table.Column<int>(type: "int", nullable: false),
                    DocumentContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentDetail", x => x.DocumentDetailId);
                    table.ForeignKey(
                        name: "FK_DocumentDetail_Document",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentVectorData",
                columns: table => new
                {
                    DocumentVectorDataId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    vector_value_id = table.Column<int>(type: "int", nullable: false),
                    vector_value = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentVectorData", x => x.DocumentVectorDataId);
                    table.ForeignKey(
                        name: "FK_DocumentVectorData_DocumentDetail",
                        column: x => x.DocumentDetailId,
                        principalTable: "DocumentDetail",
                        principalColumn: "DocumentDetailId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatHistory_UserId",
                table: "ChatHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRobotsDescription_ChatRobotId",
                table: "ChatRobotsDescription",
                column: "ChatRobotId");

            migrationBuilder.CreateIndex(
                name: "IX_Document_ChatRobotId",
                table: "Document",
                column: "ChatRobotId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentDetail_DocumentId",
                table: "DocumentDetail",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVectorData_DocumentDetailId",
                table: "DocumentVectorData",
                column: "DocumentDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatHistory");

            migrationBuilder.DropTable(
                name: "ChatRobotsDescription");

            migrationBuilder.DropTable(
                name: "DocumentVectorData");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "DocumentDetail");

            migrationBuilder.DropTable(
                name: "Document");

            migrationBuilder.DropTable(
                name: "ChatRobots");
        }
    }
}
