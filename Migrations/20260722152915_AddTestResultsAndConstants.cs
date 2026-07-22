using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NewLab.Migrations
{
    /// <inheritdoc />
    public partial class AddTestResultsAndConstants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalculationConstants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ConstantName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConstantValue = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalculationConstants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SavedComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabTestId = table.Column<int>(type: "int", nullable: false),
                    CommentText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedComments_LabTests_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientTestId = table.Column<int>(type: "int", nullable: false),
                    LabTestElementId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsAbnormal = table.Column<bool>(type: "bit", nullable: false),
                    IsCritical = table.Column<bool>(type: "bit", nullable: false),
                    FlagText = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EnteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResults_LabTestElements_LabTestElementId",
                        column: x => x.LabTestElementId,
                        principalTable: "LabTestElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TestResults_PatientTests_PatientTestId",
                        column: x => x.PatientTestId,
                        principalTable: "PatientTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CalculationConstants",
                columns: new[] { "Id", "ConstantName", "ConstantValue", "TestType", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "AgeUnder1", 8.25m, "Hgb", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Age1To12", 7.50m, "Hgb", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "MaleOver12", 6.25m, "Hgb", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "FemaleOver12", 6.75m, "Hgb", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "HctMultiplier", 3.3m, "CBC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "ISI", 1.0m, "PT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "ControlTime", 12.0m, "PT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "ControlTime", 30.0m, "PTT", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalculationConstants_TestType_ConstantName",
                table: "CalculationConstants",
                columns: new[] { "TestType", "ConstantName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedComments_LabTestId_Type",
                table: "SavedComments",
                columns: new[] { "LabTestId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_LabTestElementId",
                table: "TestResults",
                column: "LabTestElementId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_PatientTestId_LabTestElementId",
                table: "TestResults",
                columns: new[] { "PatientTestId", "LabTestElementId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalculationConstants");

            migrationBuilder.DropTable(
                name: "SavedComments");

            migrationBuilder.DropTable(
                name: "TestResults");
        }
    }
}
