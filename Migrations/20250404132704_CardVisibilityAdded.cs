using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thesis_ASP.Migrations
{
    /// <inheritdoc />
    public partial class CardVisibilityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "cardVisibility",
                table: "InGameCards",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cardVisibility",
                table: "InGameCards");
        }
    }
}
