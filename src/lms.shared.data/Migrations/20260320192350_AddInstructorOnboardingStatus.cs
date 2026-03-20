using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace lms.shared.data.Migrations
{
    /// <inheritdoc />
    public partial class AddInstructorOnboardingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstructorOnboardingStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "InProgress"),
                    LastStep = table.Column<int>(type: "integer", nullable: false),
                    YearsExperience = table.Column<int>(type: "integer", nullable: false),
                    PriorCourses = table.Column<string>(type: "text", nullable: true),
                    Specialties = table.Column<string>(type: "text", nullable: true),
                    HasRecordingEquipment = table.Column<bool>(type: "boolean", nullable: false),
                    CoursePlan = table.Column<string>(type: "text", nullable: true),
                    Headline = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    Language = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PayoutMethod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorOnboardingStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorOnboardingStatuses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstructorOnboardingStatuses_UserId",
                table: "InstructorOnboardingStatuses",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructorOnboardingStatuses");
        }
    }
}
