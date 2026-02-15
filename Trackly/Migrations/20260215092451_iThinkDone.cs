using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trackly.Migrations
{
    /// <inheritdoc />
    public partial class iThinkDone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSeed",
                table: "Habits",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSeed",
                table: "Habits");
        }
    }
}
