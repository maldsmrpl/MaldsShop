using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaldsShopWebApp.Migrations
{
    /// <inheritdoc />
    public partial class ProductItemsSold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemsSold",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemsSold",
                table: "Products");
        }
    }
}
