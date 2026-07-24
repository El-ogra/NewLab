using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewLab.Data;
using NewLab.Models.Domain;
using NewLab.Services.Interfaces;

namespace NewLab.Services.Implementations
{
    public class ReceiptSettingsService : IReceiptSettingsService
    {
        private readonly NewLabDbContext _context;

        public ReceiptSettingsService(NewLabDbContext context)
        {
            _context = context;
        }

        public async Task<ReceiptSettings> GetAsync()
        {
            return await _context.ReceiptSettings.FirstOrDefaultAsync()
                   ?? new ReceiptSettings { Id = 1, AutoPrintAfterSave = false, ShowTestsDetails = true };
        }
    }
}
