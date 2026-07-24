using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLab.Migrations
{
    /// <inheritdoc />
    public partial class AddForPregnancyOnlyToNormalRanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ForPregnancyOnly",
                table: "NormalRanges",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForPregnancyOnly",
                table: "NormalRanges");
        }
    }
}
