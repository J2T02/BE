
using Microsoft.EntityFrameworkCore;
using SWP.Controllers;
using SWP.Dtos.StepDetail;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class StepDetailRepository : IStepDetail
    {
        private readonly HIEM_MUONContext _context;

        public StepDetailRepository(HIEM_MUONContext context)
        {
            _context = context;
        }

        public async Task<StepDetail> CreateStepDetail(StepDetail stepDetailModel)
        {
             await _context.StepDetails.AddAsync(stepDetailModel);
            await _context.SaveChangesAsync();
            var result = await GetStepDetailById(stepDetailModel.SdId);
            return result;

        }

        public async Task<List<StepDetail>?> GetAllStepDetailByTreatmentPlanId(int treatmentPlanId)
        {
            return await _context.StepDetails
                .Include(x => x.Tp).ThenInclude(x => x.Cus)
                .Include(x => x.Tp).ThenInclude(x => x.Ser)
                .Include(x => x.Tp).ThenInclude(x => x.StatusNavigation)
                .Include(x => x.Ts).Include(x => x.Doc).ThenInclude(x => x.Acc)
                .Include(x => x.StatusNavigation).Where(x => x.TpId == treatmentPlanId).ToListAsync();
        }

        public async Task<StepDetail?> GetStepDetailById(int id)
        {
            return await _context.StepDetails
                .Include(x => x.Tp).ThenInclude(x => x.Cus)
                .Include(x => x.Tp).ThenInclude(x => x.Ser)
                .Include(x => x.Tp).ThenInclude(x => x.StatusNavigation)
                .Include(x => x.Ts).Include(x => x.Doc).ThenInclude(x => x.Acc)
                .Include(x => x.StatusNavigation).FirstOrDefaultAsync(x => x.SdId == id);
        }

        public async Task<StepDetailStatus?> GetStepDetailStatus(int id)
        {
            return await _context.StepDetailStatuses.FirstOrDefaultAsync(x => x.StatusId == id);
        }

        public async Task<StepDetail> UpdateStepDetail(int id, UpdateStepDetailDto request)
        {
            var exist = await GetStepDetailById(id);
            if (exist == null)
            {
                throw new Exception("Không tìm thấy chi tiết bước điều trị");
            }
            exist.TpId = request.TpId;
            exist.TsId = request.TsId;
            exist.DocId = request.DocId;
            exist.StepName = request.StepName;
            exist.Note = request.Note;
            exist.Status = request.Status;
            exist.PlanDate = request.PlanDate;
            exist.DoneDate = request.DoneDate;
            exist.DrugName = request.DrugName;
            exist.Dosage = request.Dosage;
            await _context.SaveChangesAsync();
            return exist;
        }

        public async Task<StepDetail> UpdateStepDetailStatus(int id, UpdateStatusDto request)
        {
            var existStepDetail = await GetStepDetailById(id);
            if (existStepDetail == null)
            {
                throw new Exception("Không tìm thấy chi tiết bước điều trị");
            }
            existStepDetail.Status = request.StatusId;
            _context.StepDetails.Update(existStepDetail);
            await _context.SaveChangesAsync();
            return existStepDetail;
        }
    }
}
