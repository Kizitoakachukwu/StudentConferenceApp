using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixManagersLinkedUserIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LinkedUserId",
                table: "Managers",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Managers_LinkedUserId",
                table: "Managers",
                column: "LinkedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Managers_AspNetUsers_LinkedUserId",
                table: "Managers",
                column: "LinkedUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Managers_AspNetUsers_LinkedUserId",
                table: "Managers");

            migrationBuilder.DropIndex(
                name: "IX_Managers_LinkedUserId",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "LinkedUserId",
                table: "Managers");
        }
    }
}
