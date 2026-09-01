using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace pharmamatch.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWatchlistAndLastUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryBatches_Medicines_ProductMedicineId",
                table: "InventoryBatches");

            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_ActiveIngredients_ActiveIngredientId",
                table: "Medicines");

            migrationBuilder.DropIndex(
                name: "IX_Medicines_ActiveIngredientId",
                table: "Medicines");

            migrationBuilder.DropIndex(
                name: "IX_InventoryBatches_ProductMedicineId",
                table: "InventoryBatches");

            migrationBuilder.DropColumn(
                name: "ActiveIngredientId",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "ProductMedicineId",
                table: "InventoryBatches");

            migrationBuilder.AlterColumn<string>(
                name: "TradeName",
                table: "Medicines",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Medicines",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsWatchlist",
                table: "Medicines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Medicines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "BatchNumber",
                table: "InventoryBatches",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ScientificName",
                table: "ActiveIngredients",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_IngredientId",
                table: "Medicines",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBatches_MedicineId",
                table: "InventoryBatches",
                column: "MedicineId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryBatches_Medicines_MedicineId",
                table: "InventoryBatches",
                column: "MedicineId",
                principalTable: "Medicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_ActiveIngredients_IngredientId",
                table: "Medicines",
                column: "IngredientId",
                principalTable: "ActiveIngredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryBatches_Medicines_MedicineId",
                table: "InventoryBatches");

            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_ActiveIngredients_IngredientId",
                table: "Medicines");

            migrationBuilder.DropIndex(
                name: "IX_Medicines_IngredientId",
                table: "Medicines");

            migrationBuilder.DropIndex(
                name: "IX_InventoryBatches_MedicineId",
                table: "InventoryBatches");

            migrationBuilder.DropColumn(
                name: "IsWatchlist",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Medicines");

            migrationBuilder.AlterColumn<string>(
                name: "TradeName",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Medicines",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "ActiveIngredientId",
                table: "Medicines",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BatchNumber",
                table: "InventoryBatches",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "ProductMedicineId",
                table: "InventoryBatches",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ScientificName",
                table: "ActiveIngredients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_ActiveIngredientId",
                table: "Medicines",
                column: "ActiveIngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBatches_ProductMedicineId",
                table: "InventoryBatches",
                column: "ProductMedicineId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryBatches_Medicines_ProductMedicineId",
                table: "InventoryBatches",
                column: "ProductMedicineId",
                principalTable: "Medicines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_ActiveIngredients_ActiveIngredientId",
                table: "Medicines",
                column: "ActiveIngredientId",
                principalTable: "ActiveIngredients",
                principalColumn: "Id");
        }
    }
}
