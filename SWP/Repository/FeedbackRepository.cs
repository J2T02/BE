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

        public async Task<Feedback?> GetFeedbackById(int id)
        {
            return await _context.Feedbacks
                .Include(x => x.Tp).ThenInclude(x => x.Ser)
                .Include(x => x.Tp).ThenInclude(x => x.Cus).ThenInclude(x=>x.Acc)
                .Include(x => x.Tp).ThenInclude(x => x.StatusNavigation)
                .Include(x => x.Doc).ThenInclude(x => x.Acc).ThenInclude( x=> x.Role)
                .Include(x => x.Doc).ThenInclude(x =>x.StatusNavigation)
                .Include(x => x.Doc).ThenInclude(x =>x.Edu)
                .FirstOrDefaultAsync(x => x.FbId == id);
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

        public Task<Feedback?> PostFeedback(Feedback request)
        {
            if(request == null)
            {
                throw new Exception("Không tìm thấy phản hồi yêu cầu");
            }
            _context.Feedbacks.Add(request);
            _context.SaveChanges();
            var response = GetFeedbackById(request.FbId);
            return response;
        }
    }
}
