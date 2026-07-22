using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLab.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalRanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NormalRanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabTestId = table.Column<int>(type: "int", nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TestUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    AgeFrom = table.Column<int>(type: "int", nullable: false),
                    AgeTo = table.Column<int>(type: "int", nullable: false),
                    AgeUnit = table.Column<int>(type: "int", nullable: false),
                    NormalRangeText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LowLimit = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    HighLimit = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    LowFlag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HighFlag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LowComment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HighComment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CriticalComment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CriticalRangeText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CriticalLowLimit = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CriticalHighLimit = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    CriticalFlag = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NormalRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NormalRanges_LabTests_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NormalRanges_LabTestId_Gender_AgeFrom_AgeTo_AgeUnit",
                table: "NormalRanges",
                columns: new[] { "LabTestId", "Gender", "AgeFrom", "AgeTo", "AgeUnit" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NormalRanges");
        }
    }
}
