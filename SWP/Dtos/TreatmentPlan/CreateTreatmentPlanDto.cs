using SWP.Models;

namespace SWP.Dtos.TreatmentPlan
{
    public class CreateTreatmentPlanDto
    {

        public DateOnly? EndDate { get; set; }

        public int SerId { get; set; }

        public int CusId { get; set; }

    }
}
