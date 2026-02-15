using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trackly.Migrations
{
    /// <inheritdoc />
    public partial class changeinhabits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HabitEntries_Habits_HabitId",
                table: "HabitEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Habits_Users_UserId",
                table: "Habits");

            migrationBuilder.DropIndex(
                name: "IX_Habits_UserId",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "Goal",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Habits");

            migrationBuilder.RenameColumn(
                name: "HabitId",
                table: "HabitEntries",
                newName: "UserHabitId");

            migrationBuilder.RenameIndex(
                name: "IX_HabitEntries_HabitId_Date",
                table: "HabitEntries",
                newName: "IX_HabitEntries_UserHabitId_Date");

            migrationBuilder.AddColumn<int>(
                name: "DefaultGoal",
                table: "Habits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserModelId",
                table: "Habits",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserHabits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    HabitId = table.Column<int>(type: "int", nullable: false),
                    Goal = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHabits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserHabits_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserHabits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Habits_UserModelId",
                table: "Habits",
                column: "UserModelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHabits_HabitId",
                table: "UserHabits",
                column: "HabitId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHabits_UserId_HabitId",
                table: "UserHabits",
                columns: new[] { "UserId", "HabitId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HabitEntries_UserHabits_UserHabitId",
                table: "HabitEntries",
                column: "UserHabitId",
                principalTable: "UserHabits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Habits_Users_UserModelId",
                table: "Habits",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HabitEntries_UserHabits_UserHabitId",
                table: "HabitEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Habits_Users_UserModelId",
                table: "Habits");

            migrationBuilder.DropTable(
                name: "UserHabits");

            migrationBuilder.DropIndex(
                name: "IX_Habits_UserModelId",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "DefaultGoal",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "Habits");

            migrationBuilder.RenameColumn(
                name: "UserHabitId",
                table: "HabitEntries",
                newName: "HabitId");

            migrationBuilder.RenameIndex(
                name: "IX_HabitEntries_UserHabitId_Date",
                table: "HabitEntries",
                newName: "IX_HabitEntries_HabitId_Date");

            migrationBuilder.AddColumn<int>(
                name: "Goal",
                table: "Habits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Habits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Habits_UserId",
                table: "Habits",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HabitEntries_Habits_HabitId",
                table: "HabitEntries",
                column: "HabitId",
                principalTable: "Habits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Habits_Users_UserId",
                table: "Habits",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
