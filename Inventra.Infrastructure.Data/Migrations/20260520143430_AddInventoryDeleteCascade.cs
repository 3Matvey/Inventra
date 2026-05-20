using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventra.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryDeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "fk_inventory_items_inventories_inventory_id",
                table: "inventory_items",
                column: "inventory_id",
                principalTable: "inventories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_inventory_sequences_inventories_inventory_id",
                table: "inventory_sequences",
                column: "inventory_id",
                principalTable: "inventories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_inventory_items_inventories_inventory_id",
                table: "inventory_items");

            migrationBuilder.DropForeignKey(
                name: "fk_inventory_sequences_inventories_inventory_id",
                table: "inventory_sequences");
        }
    }
}
