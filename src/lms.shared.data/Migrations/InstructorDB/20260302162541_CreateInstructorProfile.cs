using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace lms.shared.data.Migrations.InstructorDB
{
    /// <inheritdoc />
    public partial class CreateInstructorProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstructorProfile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    Headline = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Biography = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PrimaryLanguage = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Website = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Twitter = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Facebook = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LinkedIn = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    YouTube = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorProfile", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstructorProfile_ApplicationUserId",
                table: "InstructorProfile",
                column: "ApplicationUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstructorProfile");
        }
    }
}
