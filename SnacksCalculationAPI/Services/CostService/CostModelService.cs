using Microsoft.EntityFrameworkCore;
using SnacksCalculationAPI.Models;

namespace SnacksCalculationAPI.Services.CostService
{
    public class CostModelService
    {
        private readonly APIDbContext _context;
        public CostModelService(APIDbContext context)
        {
            _context = context;
        }

        public async Task<List<CostModel>> GetCostModelsByDateRangeAsync(string formDate, string toDate)
        {
            return await _context.CostModels
                                 .Where(x => x.Date == formDate && x.Date == toDate)
                                 .ToListAsync();
        }
    }
}
