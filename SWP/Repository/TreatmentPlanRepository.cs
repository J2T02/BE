using Microsoft.EntityFrameworkCore;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class TreatmentPlanRepository : ITreatmentPlan
    {
        private readonly HIEM_MUONContext _context;

        public TreatmentPlanRepository(HIEM_MUONContext context)
        {
            _context = context;
        }

        public async Task<TreatmentPlan> CreateTreatmentPlan(TreatmentPlan treatmentPlan)
        {
            await _context.TreatmentPlans.AddAsync(treatmentPlan);
            await _context.SaveChangesAsync();
            return treatmentPlan;
        }

        public async Task<TreatmentPlan> GetTreatmentPlanById(int id)
        {
            return await _context.TreatmentPlans.Include(x => x.Doc).ThenInclude(x => x.Acc).Include(x => x.Ser).Include(x => x.Cus).Include(x => x.StatusNavigation).FirstOrDefaultAsync(x => x.TpId == id);
        }
    }
}
