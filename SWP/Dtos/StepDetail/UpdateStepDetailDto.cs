namespace SWP.Dtos.StepDetail
{
    public class UpdateStepDetailDto
    {
        public int? TpId { get; set; }

        public int? TsId { get; set; }

        public int DocId { get; set; }

        public string StepName { get; set; }

        public string? Note { get; set; }

        public int Status { get; set; }

        public DateOnly? PlanDate { get; set; }

        public DateOnly? DoneDate { get; set; }

        public string? DrugName { get; set; }

        public string? Dosage { get; set; } //Lieu luong    1
    }
}
