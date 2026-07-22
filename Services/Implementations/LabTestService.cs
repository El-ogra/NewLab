using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewLab.Data;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;

namespace NewLab.Services.Implementations
{
    public class LabTestService : ILabTestService
    {
        private readonly NewLabDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public LabTestService(NewLabDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<LabTest>> GetAllAsync()
        {
            return await _context.LabTests
                .Include(l => l.TestGroup)
                .OrderBy(l => l.TestName)
                .ToListAsync();
        }

        public async Task<List<LabTest>> SearchAsync(string? code, string? groupName, string? testName)
        {
            var query = _context.LabTests
                .Include(l => l.TestGroup)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(code))
            {
                query = query.Where(l => EF.Functions.Like(l.Code, $"{code}%"));
            }

            if (!string.IsNullOrWhiteSpace(groupName))
            {
                query = query.Where(l => l.TestGroup != null && EF.Functions.Like(l.TestGroup.Name, $"%{groupName}%"));
            }

            if (!string.IsNullOrWhiteSpace(testName))
            {
                query = query.Where(l => EF.Functions.Like(l.TestName, $"%{testName}%") || EF.Functions.Like(l.ArabicName, $"%{testName}%"));
            }

            return await query.OrderBy(l => l.TestName).ToListAsync();
        }

        public async Task<LabTest?> GetByIdAsync(int labTestId)
        {
            return await _context.LabTests
                .Include(l => l.TestGroup)
                .Include(l => l.ParentLabTest)
                .Include(l => l.DefaultSpecimenType)
                .Include(l => l.ExternalReferral)
                .Include(l => l.Elements)
                .Include(l => l.ReferralPrices)
                    .ThenInclude(rp => rp.Referral)
                .FirstOrDefaultAsync(l => l.Id == labTestId);
        }

        public async Task<LabTest> AddAsync(LabTest labTest)
        {
            _context.LabTests.Add(labTest);
            await _context.SaveChangesAsync();
            return labTest;
        }

        public async Task<LabTest> UpdateAsync(LabTest labTest)
        {
            _context.LabTests.Update(labTest);
            await _context.SaveChangesAsync();
            return labTest;
        }

        public async Task DeleteAsync(int labTestId)
        {
            if (!_currentUserService.IsAdmin)
            {
                throw new UnauthorizedAccessException("عملية الحذف تتطلب صلاحية Admin");
            }

            var labTest = await _context.LabTests.FindAsync(labTestId);
            if (labTest != null)
            {
                _context.LabTests.Remove(labTest);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<LabTestElement>> GetElementsAsync(int parentLabTestId)
        {
            return await _context.LabTestElements
                .Where(e => e.ParentLabTestId == parentLabTestId)
                .OrderBy(e => e.ArrangeNumber)
                .ToListAsync();
        }

        public async Task<List<LabTest>> GetRoutineTestsAsync()
        {
            return await _context.LabTests
                .Where(l => l.IsRoutine && l.IsActive)
                .Include(l => l.TestGroup)
                .OrderBy(l => l.TestName)
                .ToListAsync();
        }

        public async Task<List<LabTest>> GetByGroupAsync(int testGroupId)
        {
            return await _context.LabTests
                .Where(l => l.TestGroupId == testGroupId && l.IsActive)
                .Include(l => l.TestGroup)
                .OrderBy(l => l.TestName)
                .ToListAsync();
        }

        public async Task<List<ReferralPrice>> GetReferralPricesAsync(int labTestId)
        {
            return await _context.ReferralPrices
                .Where(rp => rp.LabTestId == labTestId)
                .Include(rp => rp.Referral)
                .ToListAsync();
        }

        public async Task SetReferralPriceAsync(int labTestId, int referralId, decimal price)
        {
            var existing = await _context.ReferralPrices
                .FirstOrDefaultAsync(rp => rp.LabTestId == labTestId && rp.ReferralId == referralId);

            if (existing != null)
            {
                existing.Price = price;
            }
            else
            {
                _context.ReferralPrices.Add(new ReferralPrice
                {
                    LabTestId = labTestId,
                    ReferralId = referralId,
                    Price = price
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteReferralPriceAsync(int labTestId, int referralId)
        {
            var existing = await _context.ReferralPrices
                .FirstOrDefaultAsync(rp => rp.LabTestId == labTestId && rp.ReferralId == referralId);

            if (existing != null)
            {
                _context.ReferralPrices.Remove(existing);
                await _context.SaveChangesAsync();
            }
        }
    }
}
