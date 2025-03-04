using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thesis_ASP.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewColumnOnInGameCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "currentParent",
                table: "InGameCards",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "currentParent",
                table: "InGameCards");
        }
    }
}
