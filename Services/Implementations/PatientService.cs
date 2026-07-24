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
    public class PatientService : IPatientService
    {
        private readonly NewLabDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public PatientService(NewLabDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Patient> AddAsync(Patient patient)
        {
            patient.CreatedAt = DateTime.Now;
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<Patient> UpdateAsync(Patient patient)
        {
            patient.UpdatedAt = DateTime.Now;
            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task DeleteAsync(int patientId)
        {
            if (!_currentUserService.IsAdmin)
            {
                throw new UnauthorizedAccessException("عملية الحذف تتطلب صلاحية Admin");
            }

            var patient = await _context.Patients.FindAsync(patientId);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Patient?> GetByIdAsync(int patientId)
        {
            return await _context.Patients
                .Include(p => p.Referral)
                .Include(p => p.ExternalSpecimenType)
                .Include(p => p.CreatedByUser)
                .Include(p => p.Visits)
                .FirstOrDefaultAsync(p => p.Id == patientId);
        }

        public async Task<List<Patient>> GetTodayPatientsAsync()
        {
            var today = DateTime.Today;
            return await _context.Patients
                .Where(p => p.CreatedAt >= today && p.CreatedAt < today.AddDays(1))
                .Include(p => p.Referral)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Patient?> GetByLabIdAsync(string labId)
        {
            return await _context.Patients
                .Include(p => p.Referral)
                .FirstOrDefaultAsync(p => p.LabId == labId);
        }

        public async Task<decimal> CalculateTotalAsync(IEnumerable<PatientTestRow> patientTests, BillingSystem billingSystem, Referral? referral, decimal discountValue, bool discountIsPercent)
        {
            decimal total = 0;

            foreach (var test in patientTests)
            {
                switch (billingSystem)
                {
                    case BillingSystem.Free:
                        break;
                    case BillingSystem.LabToLab:
                        decimal priceForThisTest = test.Price;
                        if (referral != null)
                        {
                            var referralPrice = await _context.ReferralPrices
                                .FirstOrDefaultAsync(rp => rp.LabTestId == test.LabTestId
                                                        && rp.ReferralId == referral.Id);
                            if (referralPrice != null)
                            {
                                priceForThisTest = referralPrice.Price;
                            }
                            else
                            {
                                // Fallback to LabToLabPrice from LabTest entity
                                var labTest = await _context.LabTests.FindAsync(test.LabTestId);
                                if (labTest != null)
                                    priceForThisTest = labTest.LabToLabPrice;
                            }
                        }
                        else
                        {
                            var labTest = await _context.LabTests.FindAsync(test.LabTestId);
                            if (labTest != null)
                                priceForThisTest = labTest.LabToLabPrice;
                        }
                        total += priceForThisTest;
                        break;
                    case BillingSystem.Individual:
                    default:
                        total += test.Price;
                        break;
                }
            }

            // Apply referral discount
            if (referral != null && referral.DiscountPercent > 0)
            {
                total -= total * (referral.DiscountPercent / 100);
            }

            // Apply patient discount
            if (discountValue > 0)
            {
                if (discountIsPercent)
                {
                    total -= total * (discountValue / 100);
                }
                else
                {
                    total -= discountValue;
                }
            }

            return Math.Max(0, total);
        }
    }
}
