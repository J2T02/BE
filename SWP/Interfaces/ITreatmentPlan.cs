using SWP.Dtos.TreatmentPlan;
using SWP.Models;

namespace SWP.Interfaces
{
    public interface ITreatmentPlan
    {
        Task<List<TreatmentPlan>?> GetAllTreatmentPlans();
        Task<TreatmentPlan> CreateTreatmentPlan(TreatmentPlan treatmentPlan);
        Task<TreatmentPlan?> GetTreatmentPlanById(int id);
        Task<TreatmentStep> CreateTreatmentStep(TreatmentStep treatmentStep);
        Task<TreatmentStep?> GetTreatmentStepById(int id);
        Task<TreatmentPlanStatus?> GetTreatmentPlanStatus(int id);
        Task<TreatmentPlan?> UpdateTreatmentPlan(int id, UpdateTreatmentPlanDto request);
        Task<List<TreatmentPlan>?> GetTreatmentPlanByCustomerId(int customerId);
        Task<List<TreatmentPlan>?> GetTreatmentPlanByDoctorId(int doctorId);
    }
}
