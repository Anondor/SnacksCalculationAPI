using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnacksCalculationAPI.Migrations
{
    /// <inheritdoc />
    public partial class updatedatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCostInfo",
                table: "UserCostInfo");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "UserCostInfo");

            migrationBuilder.RenameTable(
                name: "UserCostInfo",
                newName: "AccountTable");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountTable",
                table: "AccountTable",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CostInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Item = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostInfo", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostInfo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountTable",
                table: "AccountTable");

            migrationBuilder.RenameTable(
                name: "AccountTable",
                newName: "UserCostInfo");

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "UserCostInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCostInfo",
                table: "UserCostInfo",
                column: "Id");
        }
    }
}
