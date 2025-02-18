using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thesis_ASP.Migrations
{
    /// <inheritdoc />
    public partial class AddAttributeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_cards_name",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "name",
                table: "cards");

            migrationBuilder.AddColumn<int>(
                name: "attribute",
                table: "cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "cardID",
                table: "cards",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "cardName",
                table: "cards",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "cardType",
                table: "cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "characterType",
                table: "cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "color",
                table: "cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "cost",
                table: "cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "counter",
                table: "cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "effect",
                table: "cards",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "power",
                table: "cards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "trigger",
                table: "cards",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "cards",
                columns: new[] { "id", "attribute", "cardID", "cardName", "cardType", "characterType", "color", "cost", "counter", "effect", "power", "trigger" },
                values: new object[] { 2L, 0, "ST01-002", "Usopp", 0, 0, 4, 2, 1000, "effect", 2000, "trigger" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "cards",
                keyColumn: "id",
                keyValue: 2L);

            migrationBuilder.DropColumn(
                name: "attribute",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "cardID",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "cardName",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "cardType",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "characterType",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "color",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "cost",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "counter",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "effect",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "power",
                table: "cards");

            migrationBuilder.DropColumn(
                name: "trigger",
                table: "cards");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "cards",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_cards_name",
                table: "cards",
                column: "name");
        }
    }
}
