using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLab.Migrations
{
    /// <inheritdoc />
    public partial class AddLabTestSpecimensAndLabelName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "AgeValue",
                table: "Patients",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "LabelName",
                table: "LabTests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LabTestSpecimens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabTestId = table.Column<int>(type: "int", nullable: false),
                    SpecimenTypeId = table.Column<int>(type: "int", nullable: false),
                    TubeOrder = table.Column<int>(type: "int", nullable: false),
                    LabelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTestSpecimens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTestSpecimens_LabTests_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabTestSpecimens_SpecimenTypes_SpecimenTypeId",
                        column: x => x.SpecimenTypeId,
                        principalTable: "SpecimenTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "LabTests",
                keyColumn: "Id",
                keyValue: 1,
                column: "LabelName",
                value: null);

            migrationBuilder.UpdateData(
                table: "LabTests",
                keyColumn: "Id",
                keyValue: 2,
                column: "LabelName",
                value: null);

            migrationBuilder.UpdateData(
                table: "LabTests",
                keyColumn: "Id",
                keyValue: 3,
                column: "LabelName",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_LabTestSpecimens_LabTestId",
                table: "LabTestSpecimens",
                column: "LabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestSpecimens_SpecimenTypeId",
                table: "LabTestSpecimens",
                column: "SpecimenTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabTestSpecimens");

            migrationBuilder.DropColumn(
                name: "LabelName",
                table: "LabTests");

            migrationBuilder.AlterColumn<decimal>(
                name: "AgeValue",
                table: "Patients",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");
        }
    }
}
