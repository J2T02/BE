using SWP.Models;

namespace SWP.Interfaces
{
    public interface ITreatmentPlan
    {
        Task<TreatmentPlan> CreateTreatmentPlan(TreatmentPlan treatmentPlan);
        Task<TreatmentPlan> GetTreatmentPlanById(int id);
    }
}
