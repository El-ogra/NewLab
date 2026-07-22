using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NewLab.Migrations
{
    /// <inheritdoc />
    public partial class AddLabTestsAndReferralPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LogGroup = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LabTests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TestName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReportNameLarge = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReportNameSmall = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BillNameLarge = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BillNameSmall = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HistoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TestGroupId = table.Column<int>(type: "int", nullable: true),
                    LogGroup = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Collection = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestTimeDays = table.Column<int>(type: "int", nullable: false),
                    ArrangeNumber = table.Column<int>(type: "int", nullable: false),
                    PatientPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LabToLabPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsRoutine = table.Column<bool>(type: "bit", nullable: false),
                    IsSeeReport = table.Column<bool>(type: "bit", nullable: false),
                    IsPrintWithOther = table.Column<bool>(type: "bit", nullable: false),
                    IsAddWithGroup = table.Column<bool>(type: "bit", nullable: false),
                    IsMainTest = table.Column<bool>(type: "bit", nullable: false),
                    ParentLabTestId = table.Column<int>(type: "int", nullable: true),
                    DefaultSpecimenTypeId = table.Column<int>(type: "int", nullable: true),
                    IsSentExternal = table.Column<bool>(type: "bit", nullable: false),
                    ExternalReferralId = table.Column<int>(type: "int", nullable: true),
                    ExternalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PromptQuestion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTests_LabTests_ParentLabTestId",
                        column: x => x.ParentLabTestId,
                        principalTable: "LabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTests_Referrals_ExternalReferralId",
                        column: x => x.ExternalReferralId,
                        principalTable: "Referrals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTests_SpecimenTypes_DefaultSpecimenTypeId",
                        column: x => x.DefaultSpecimenTypeId,
                        principalTable: "SpecimenTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTests_TestGroups_TestGroupId",
                        column: x => x.TestGroupId,
                        principalTable: "TestGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabTestElements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentLabTestId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ArrangeNumber = table.Column<int>(type: "int", nullable: false),
                    IsMainTest = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTestElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTestElements_LabTests_ParentLabTestId",
                        column: x => x.ParentLabTestId,
                        principalTable: "LabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReferralPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabTestId = table.Column<int>(type: "int", nullable: false),
                    ReferralId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferralPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferralPrices_LabTests_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReferralPrices_Referrals_ReferralId",
                        column: x => x.ReferralId,
                        principalTable: "Referrals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "TestGroups",
                columns: new[] { "Id", "LogGroup", "Name" },
                values: new object[,]
                {
                    { 1, "CHEM", "Chemistry" },
                    { 2, "HEM", "Hematology" },
                    { 3, "URI", "Urine" }
                });

            migrationBuilder.InsertData(
                table: "LabTests",
                columns: new[] { "Id", "ArabicName", "ArrangeNumber", "BillNameLarge", "BillNameSmall", "Code", "Collection", "DefaultSpecimenTypeId", "ExternalCost", "ExternalReferralId", "HistoryName", "IsActive", "IsAddWithGroup", "IsMainTest", "IsPrintWithOther", "IsRoutine", "IsSeeReport", "IsSentExternal", "LabToLabPrice", "LogGroup", "ParentLabTestId", "PatientPrice", "PromptQuestion", "ReportNameLarge", "ReportNameSmall", "TestGroupId", "TestName", "TestTimeDays" },
                values: new object[,]
                {
                    { 1, "سكر", 1, null, null, "GLU", null, null, null, null, null, true, false, false, false, true, false, false, 30m, null, null, 10m, null, "Glucose", null, 1, "Glucose", 0 },
                    { 2, "هموغلوبين", 2, null, null, "HGB", null, null, null, null, null, true, false, false, false, true, false, false, 35m, null, null, 15m, null, "Hemoglobin", null, 2, "Hemoglobin", 0 },
                    { 3, "تحليل بول", 3, null, null, "UAC", null, null, null, null, null, true, false, false, false, true, false, false, 20m, null, null, 8m, null, "Urine Analysis", null, 3, "Urine Analysis", 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LabTestElements_ParentLabTestId",
                table: "LabTestElements",
                column: "ParentLabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_Code",
                table: "LabTests",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_DefaultSpecimenTypeId",
                table: "LabTests",
                column: "DefaultSpecimenTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_ExternalReferralId",
                table: "LabTests",
                column: "ExternalReferralId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_ParentLabTestId",
                table: "LabTests",
                column: "ParentLabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_TestGroupId",
                table: "LabTests",
                column: "TestGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferralPrices_LabTestId_ReferralId",
                table: "ReferralPrices",
                columns: new[] { "LabTestId", "ReferralId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferralPrices_ReferralId",
                table: "ReferralPrices",
                column: "ReferralId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LabTestElements");

            migrationBuilder.DropTable(
                name: "ReferralPrices");

            migrationBuilder.DropTable(
                name: "LabTests");

            migrationBuilder.DropTable(
                name: "TestGroups");
        }
    }
}
