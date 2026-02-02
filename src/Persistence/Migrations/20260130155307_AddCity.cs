using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MediaUrl",
                table: "PropertyMedias",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MediaType",
                table: "PropertyMedias",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "PropertyAds",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PropertyAds",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "AreaInSquareMeters",
                table: "PropertyAds",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyMedias_PropertyAdId_Order",
                table: "PropertyMedias",
                columns: new[] { "PropertyAdId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAds_CreatedAt",
                table: "PropertyAds",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAds_OfferType",
                table: "PropertyAds",
                column: "OfferType");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAds_OfferType_PropertyCategory",
                table: "PropertyAds",
                columns: new[] { "OfferType", "PropertyCategory" });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAds_OfferType_PropertyCategory_Price",
                table: "PropertyAds",
                columns: new[] { "OfferType", "PropertyCategory", "Price" });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAds_Price",
                table: "PropertyAds",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAds_PropertyCategory",
                table: "PropertyAds",
                column: "PropertyCategory");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAds_RoomCount",
                table: "PropertyAds",
                column: "RoomCount");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_Name",
                table: "Cities",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_PropertyMedias_PropertyAdId_Order",
                table: "PropertyMedias");

            migrationBuilder.DropIndex(
                name: "IX_PropertyAds_CreatedAt",
                table: "PropertyAds");

            migrationBuilder.DropIndex(
                name: "IX_PropertyAds_OfferType",
                table: "PropertyAds");

            migrationBuilder.DropIndex(
                name: "IX_PropertyAds_OfferType_PropertyCategory",
                table: "PropertyAds");

            migrationBuilder.DropIndex(
                name: "IX_PropertyAds_OfferType_PropertyCategory_Price",
                table: "PropertyAds");

            migrationBuilder.DropIndex(
                name: "IX_PropertyAds_Price",
                table: "PropertyAds");

            migrationBuilder.DropIndex(
                name: "IX_PropertyAds_PropertyCategory",
                table: "PropertyAds");

            migrationBuilder.DropIndex(
                name: "IX_PropertyAds_RoomCount",
                table: "PropertyAds");

            migrationBuilder.AlterColumn<string>(
                name: "MediaUrl",
                table: "PropertyMedias",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "MediaType",
                table: "PropertyMedias",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "PropertyAds",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PropertyAds",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<decimal>(
                name: "AreaInSquareMeters",
                table: "PropertyAds",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");
        }
    }
}
