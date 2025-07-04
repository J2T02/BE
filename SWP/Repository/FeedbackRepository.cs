using Microsoft.EntityFrameworkCore;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly HIEM_MUONContext _context;
        public FeedbackRepository(HIEM_MUONContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByDoctorIdAsync(int doctorId)
        {
            return await _context.Feedbacks
                .Include(f => f.Tp)
                .ThenInclude(tp => tp.Cus)
                .ThenInclude(cus => cus.Acc)
                .Where(f => f.DocId == doctorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByTreatmentPlanIdAsync(int treatmentPlanId)
        {
            return await _context.Feedbacks
                .Include(f => f.Tp)
                .ThenInclude(tp => tp.Cus)
                .ThenInclude(cus => cus.Acc)
                .Where(f => f.TpId == treatmentPlanId)
                .ToListAsync();
        }
    }
}
