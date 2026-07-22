using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLab.Migrations
{
    /// <inheritdoc />
    public partial class AddBarcodeSettingsAndPatientCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BarcodeSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OffsetX = table.Column<int>(type: "int", nullable: false),
                    OffsetY = table.Column<int>(type: "int", nullable: false),
                    PrintFileCodeWithAll = table.Column<bool>(type: "bit", nullable: false),
                    LabelWidth = table.Column<int>(type: "int", nullable: false),
                    LabelHeight = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarcodeSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatientCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    CodeType = table.Column<int>(type: "int", nullable: false),
                    CodeValue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientCodes_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BarcodeSettings",
                columns: new[] { "Id", "LabelHeight", "LabelWidth", "OffsetX", "OffsetY", "PrintFileCodeWithAll" },
                values: new object[] { 1, 25, 38, 0, 0, false });

            migrationBuilder.CreateIndex(
                name: "IX_PatientCodes_CodeValue",
                table: "PatientCodes",
                column: "CodeValue");

            migrationBuilder.CreateIndex(
                name: "IX_PatientCodes_PatientId_CodeType",
                table: "PatientCodes",
                columns: new[] { "PatientId", "CodeType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarcodeSettings");

            migrationBuilder.DropTable(
                name: "PatientCodes");
        }
    }
}
