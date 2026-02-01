using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lms.shared.data.Migrations.CourseDb
{
    /// <inheritdoc />
    public partial class AddUploadedToCourseModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Uploaded",
                table: "CourseModules",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "CourseModules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FileUploadQueueItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileBytes = table.Column<byte[]>(type: "bytea", nullable: true),
                    QueueStatus = table.Column<string>(type: "text", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    QueuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileUploadQueueItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileUploadQueueItems_CourseModules_CourseModuleId",
                        column: x => x.CourseModuleId,
                        principalTable: "CourseModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileUploadQueueItems_CourseModuleId",
                table: "FileUploadQueueItems",
                column: "CourseModuleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileUploadQueueItems");

            migrationBuilder.DropColumn(
                name: "Uploaded",
                table: "CourseModules");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "CourseModules");
        }
    }
}
