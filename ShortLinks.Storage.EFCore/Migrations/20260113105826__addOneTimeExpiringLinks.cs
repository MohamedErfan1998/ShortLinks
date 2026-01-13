using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShortLinks.Storage.EFCore.Migrations
{
    /// <inheritdoc />
    public partial class _addOneTimeExpiringLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxUses",
                table: "ShortLinks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsedCount",
                table: "ShortLinks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxUses",
                table: "ShortLinks");

            migrationBuilder.DropColumn(
                name: "UsedCount",
                table: "ShortLinks");
        }
    }
}
