namespace SWP.Dtos.TreatmentPlan
{
    public class UpdateTreatmentPlanDto
    {
        public int SerId { get; set; }
        public int Status { get; set; }
        public  DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Result { get; set; } = string.Empty;

    }
}
