using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewLab.Data;
using NewLab.Models.Domain;
using NewLab.Models.Domain.Enums;
using NewLab.Services.Interfaces;

namespace NewLab.Services.Implementations
{
    public class BarcodeService : IBarcodeService
    {
        private const string BranchConstant = "1";

        private readonly NewLabDbContext _context;

        public BarcodeService(NewLabDbContext context)
        {
            _context = context;
        }

        public string GenerateCaseCode(Patient patient, PatientVisit visit)
        {
            return BuildCode13(CodeType.Case, visit.DailySequenceNumber);
        }

        public string GenerateFileCode(Patient patient)
        {
            // Use patient id as a simple sequence for file codes
            var sequence = patient.Id;
            return BuildCode13(CodeType.File, sequence);
        }

        public string GenerateLabCode(Patient patient)
        {
            var sequence = patient.Id;
            return BuildCode13(CodeType.Lab, sequence);
        }

        public async Task<Patient> GetOrCreateLabIdAsync(Patient patient)
        {
            if (!string.IsNullOrEmpty(patient.LabId))
                return patient;

            var code = GenerateLabCode(patient);
            patient.LabId = code;

            _context.Patients.Update(patient);

            // Also create PatientCode record
            var existingCode = await _context.PatientCodes
                .FirstOrDefaultAsync(pc => pc.PatientId == patient.Id && pc.CodeType == CodeType.Lab);

            if (existingCode == null)
            {
                _context.PatientCodes.Add(new PatientCode
                {
                    PatientId = patient.Id,
                    CodeType = CodeType.Lab,
                    CodeValue = code,
                    IssuedAt = DateTime.Now
                });
            }
            else
            {
                existingCode.CodeValue = code;
                existingCode.IssuedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<List<BarcodeLabel>> GetLabelsForPatientAsync(int patientId)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (patient == null) return new List<BarcodeLabel>();

            var labels = new List<BarcodeLabel>();

            // For now, create one label for all tests (will be expanded with F3)
            if (!string.IsNullOrEmpty(patient.LabId))
            {
                var caseVisit = await _context.PatientVisits
                    .Where(v => v.PatientId == patientId)
                    .OrderByDescending(v => v.VisitDate)
                    .FirstOrDefaultAsync();

                if (caseVisit != null)
                {
                    labels.Add(new BarcodeLabel
                    {
                        PatientId = patientId,
                        PatientName = patient.FullName,
                        SpecimenName = "Default",
                        Code = GenerateCaseCode(patient, caseVisit),
                        Type = CodeType.Case
                    });
                }
            }

            return labels;
        }

        public async Task<BarcodeSettings> GetSettingsAsync()
        {
            return await _context.BarcodeSettings.FirstOrDefaultAsync()
                ?? new BarcodeSettings { LabelWidth = 38, LabelHeight = 25 };
        }

        public async Task SaveSettingsAsync(BarcodeSettings settings)
        {
            var existing = await _context.BarcodeSettings.FirstOrDefaultAsync();
            if (existing != null)
            {
                existing.OffsetX = settings.OffsetX;
                existing.OffsetY = settings.OffsetY;
                existing.PrintFileCodeWithAll = settings.PrintFileCodeWithAll;
                existing.LabelWidth = settings.LabelWidth;
                existing.LabelHeight = settings.LabelHeight;
            }
            else
            {
                _context.BarcodeSettings.Add(settings);
            }

            await _context.SaveChangesAsync();
        }

        private static string BuildCode13(CodeType type, int sequence)
        {
            var today = DateTime.Today;
            string datePart = today.ToString("yyMMdd");

            int typeDigit = type switch
            {
                CodeType.Case => 1,
                CodeType.File => 3,
                CodeType.Lab => 5,
                _ => 1
            };

            int dayOfWeek = ((int)today.DayOfWeek) + 1; // 1-7
            string seqPart = (sequence % 1000).ToString("D3");

            // Format: 1(padding) - typeDigit - YYMMDD - BranchConstant - sequence - dayOfWeek
            // Total: 1 + 1 + 6 + 1 + 3 + 1 = 13 chars
            return $"1{typeDigit}{datePart}{BranchConstant}{seqPart}{dayOfWeek}";
        }
    }
}
