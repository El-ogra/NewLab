using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewLab.Data;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;

namespace NewLab.Services.Implementations
{
    public class ReferralService : IReferralService
    {
        private readonly NewLabDbContext _context;

        public ReferralService(NewLabDbContext context)
        {
            _context = context;
        }

        public async Task<List<Referral>> SearchByNameAsync(string prefix)
        {
            return await _context.Referrals
                .Where(r => EF.Functions.Like(r.Name, $"{prefix}%"))
                .OrderBy(r => r.Name)
                .Take(20)
                .ToListAsync();
        }

        public async Task<Referral> GetOrCreateAsync(string name)
        {
            var existing = await _context.Referrals.FirstOrDefaultAsync(r => r.Name == name);
            if (existing != null)
            {
                return existing;
            }

            var referral = new Referral
            {
                Name = name,
                DiscountPercent = 0,
                IsDefaultLab = false,
                CreatedAt = System.DateTime.Now
            };

            _context.Referrals.Add(referral);
            await _context.SaveChangesAsync();
            return referral;
        }

        public async Task<Referral> GetDefaultLabAsync()
        {
            var defaultLab = await _context.Referrals.FirstOrDefaultAsync(r => r.IsDefaultLab);
            if (defaultLab == null)
            {
                throw new System.Exception("Default lab referral not found");
            }
            return defaultLab;
        }
    }
}
