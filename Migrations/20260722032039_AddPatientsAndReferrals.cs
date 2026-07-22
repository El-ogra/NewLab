using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewLab.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientsAndReferrals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Referrals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsDefaultLab = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Referrals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpecimenTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ArabicName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecimenTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    AgeValue = table.Column<int>(type: "int", nullable: false),
                    AgeUnit = table.Column<int>(type: "int", nullable: false),
                    BillingSystem = table.Column<int>(type: "int", nullable: false),
                    IsImportant = table.Column<bool>(type: "bit", nullable: false),
                    ReferralId = table.Column<int>(type: "int", nullable: true),
                    ReferralHiddenOnReport = table.Column<bool>(type: "bit", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NationalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LabId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FileCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VisitCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExternalSpecimenTypeId = table.Column<int>(type: "int", nullable: true),
                    DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountIsPercent = table.Column<bool>(type: "bit", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsFasting = table.Column<bool>(type: "bit", nullable: false),
                    FastingHours = table.Column<int>(type: "int", nullable: true),
                    IsOnAnticoagulant = table.Column<bool>(type: "bit", nullable: false),
                    HasLiverTreatment = table.Column<bool>(type: "bit", nullable: false),
                    HasAntiviralTreatment = table.Column<bool>(type: "bit", nullable: false),
                    HasAntibiotic = table.Column<bool>(type: "bit", nullable: false),
                    IsPregnant = table.Column<bool>(type: "bit", nullable: false),
                    IsSmoker = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patients_Referrals_ReferralId",
                        column: x => x.ReferralId,
                        principalTable: "Referrals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Patients_SpecimenTypes_ExternalSpecimenTypeId",
                        column: x => x.ExternalSpecimenTypeId,
                        principalTable: "SpecimenTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Patients_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientVisits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DailySequenceNumber = table.Column<int>(type: "int", nullable: false),
                    FullVisitCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientVisits_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Referrals",
                columns: new[] { "Id", "CreatedAt", "DiscountPercent", "IsDefaultLab", "Name" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 0m, true, "المعمل" });

            migrationBuilder.CreateIndex(
                name: "IX_Patients_CreatedByUserId",
                table: "Patients",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ExternalSpecimenTypeId",
                table: "Patients",
                column: "ExternalSpecimenTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_FileCode",
                table: "Patients",
                column: "FileCode",
                unique: true,
                filter: "[FileCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_LabId",
                table: "Patients",
                column: "LabId",
                unique: true,
                filter: "[LabId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ReferralId",
                table: "Patients",
                column: "ReferralId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_PatientId",
                table: "PatientVisits",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientVisits");

            migrationBuilder.DropTable(
                name: "Patients");

            migrationBuilder.DropTable(
                name: "Referrals");

            migrationBuilder.DropTable(
                name: "SpecimenTypes");
        }
    }
}
