using SWP.Dtos.StepDetail;
using SWP.Models;

namespace SWP.Interfaces
{
    public interface IStepDetail
    {
        Task<StepDetail> CreateStepDetail(StepDetail stepDetail);
        Task<StepDetail> GetStepDetailById(int id); 
        Task<List<StepDetail>> GetAllStepDetailByTreatmentPlanId(int treatmentPlanId);
        Task<StepDetail> UpdateStepDetail(int id, UpdateStepDetailDto request);
        Task<StepDetailStatus> GetStepDetailStatus(int id);
        Task<StepDetail> UpdateStepDetailStatus(int id, UpdateStatusDto request);
    }
}
