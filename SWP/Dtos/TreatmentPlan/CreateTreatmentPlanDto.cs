using SWP.Models;

namespace SWP.Dtos.TreatmentPlan
{
    public class CreateTreatmentPlanDto
    {
        public int DocId { get; set; }

        //public DateOnly? EndDate { get; set; }

        public int SerId { get; set; }

        public int CusId { get; set; }

    }
}
