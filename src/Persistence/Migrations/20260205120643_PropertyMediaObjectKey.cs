using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PropertyMediaObjectKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MediaType",
                table: "PropertyMedias");

            migrationBuilder.RenameColumn(
                name: "MediaUrl",
                table: "PropertyMedias",
                newName: "ObjectKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ObjectKey",
                table: "PropertyMedias",
                newName: "MediaUrl");

            migrationBuilder.AddColumn<string>(
                name: "MediaType",
                table: "PropertyMedias",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
