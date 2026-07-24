using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLab.Migrations
{
    /// <inheritdoc />
    public partial class AddTestResultReviewedPrinted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrinted",
                table: "TestResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "TestResults",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrinted",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "TestResults");
        }
    }
}
