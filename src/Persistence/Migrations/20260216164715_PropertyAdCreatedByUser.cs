using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PropertyAdCreatedByUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "PropertyAds",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAds_CreatedByUserId",
                table: "PropertyAds",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyAds_AspNetUsers_CreatedByUserId",
                table: "PropertyAds",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyAds_AspNetUsers_CreatedByUserId",
                table: "PropertyAds");

            migrationBuilder.DropIndex(
                name: "IX_PropertyAds_CreatedByUserId",
                table: "PropertyAds");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "PropertyAds");
        }
    }
}
