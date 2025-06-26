using SWP.Dtos.TreatmentPlan;
using SWP.Models;

namespace SWP.Interfaces
{
    public interface ITreatmentPlan
    {
        Task<TreatmentPlan> CreateTreatmentPlan(TreatmentPlan treatmentPlan);
        Task<TreatmentPlan?> GetTreatmentPlanById(int id);
        Task<TreatmentStep> CreateTreatmentStep(TreatmentStep treatmentStep);
        Task<TreatmentStep?> GetTreatmentStepById(int id);
        Task<TreatmentPlanStatus?> GetTreatmentPlanStatus(int id);
        Task<TreatmentPlan?> UpdateTreatmentPlan(int id, UpdateTreatmentPlanDto request);
        Task<TreatmentPlan?> GetTreatmentPlanByCustomerId(int customerId);
    }
}
