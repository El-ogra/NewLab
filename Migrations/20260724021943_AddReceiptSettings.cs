using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLab.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiptSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReceiptSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AutoPrintAfterSave = table.Column<bool>(type: "bit", nullable: false),
                    ShowTestsDetails = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ReceiptSettings",
                columns: new[] { "Id", "AutoPrintAfterSave", "ShowTestsDetails" },
                values: new object[] { 1, false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptSettings");
        }
    }
}
