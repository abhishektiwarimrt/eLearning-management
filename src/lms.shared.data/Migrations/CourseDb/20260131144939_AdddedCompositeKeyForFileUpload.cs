using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lms.shared.data.Migrations.CourseDb
{
    /// <inheritdoc />
    public partial class AdddedCompositeKeyForFileUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FileUploadQueueItems",
                table: "FileUploadQueueItems");

            migrationBuilder.DropIndex(
                name: "IX_FileUploadQueueItems_CourseModuleId",
                table: "FileUploadQueueItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileUploadQueueItems",
                table: "FileUploadQueueItems",
                columns: new[] { "CourseModuleId", "FileName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FileUploadQueueItems",
                table: "FileUploadQueueItems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileUploadQueueItems",
                table: "FileUploadQueueItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploadQueueItems_CourseModuleId",
                table: "FileUploadQueueItems",
                column: "CourseModuleId");
        }
    }
}
