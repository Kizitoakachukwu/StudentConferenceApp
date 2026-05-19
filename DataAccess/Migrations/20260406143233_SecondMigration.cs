using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Sections_SectionId",
                table: "Participants");

            migrationBuilder.DropTable(
                name: "GeneratedDocuments");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "UploadedFiles");

            migrationBuilder.DropIndex(
                name: "IX_Participants_SectionId",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "SectionId",
                table: "Participants");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Participants",
                newName: "University");

            migrationBuilder.RenameColumn(
                name: "PaperTitle",
                table: "Participants",
                newName: "SectionName");

            migrationBuilder.RenameColumn(
                name: "Institution",
                table: "Participants",
                newName: "PhoneNumber");

            migrationBuilder.AddColumn<string>(
                name: "CertificatePath",
                table: "Participants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "Participants",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvitationPath",
                table: "Participants",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CertificatePath",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "InvitationPath",
                table: "Participants");

            migrationBuilder.RenameColumn(
                name: "University",
                table: "Participants",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "SectionName",
                table: "Participants",
                newName: "PaperTitle");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Participants",
                newName: "Institution");

            migrationBuilder.AddColumn<int>(
                name: "SectionId",
                table: "Participants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GeneratedDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedDocuments_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadedFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadedFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadedFiles_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Participants_SectionId",
                table: "Participants",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedDocuments_ParticipantId",
                table: "GeneratedDocuments",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadedFiles_ParticipantId",
                table: "UploadedFiles",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Sections_SectionId",
                table: "Participants",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
