using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookify.Migrations
{
    /// <inheritdoc />
    public partial class nv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "LivreLu",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LivreLu_UserId",
                table: "LivreLu",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LivreLu_users_UserId",
                table: "LivreLu",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LivreLu_users_UserId",
                table: "LivreLu");

            migrationBuilder.DropIndex(
                name: "IX_LivreLu_UserId",
                table: "LivreLu");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LivreLu");
        }
    }
}
