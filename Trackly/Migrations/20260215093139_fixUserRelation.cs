using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trackly.Migrations
{
    /// <inheritdoc />
    public partial class fixUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habits_Users_UserModelId",
                table: "Habits");

            migrationBuilder.DropIndex(
                name: "IX_Habits_UserModelId",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "Habits");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserModelId",
                table: "Habits",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Habits_UserModelId",
                table: "Habits",
                column: "UserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Habits_Users_UserModelId",
                table: "Habits",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
