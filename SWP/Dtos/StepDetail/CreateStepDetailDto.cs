namespace SWP.Dtos.StepDetail
{
    public class CreateStepDetailDto
    {
        //public int SdId { get; set; }

        public int TpId { get; set; }

        public int TsId { get; set; }

        //public int? DocId { get; set; } get from Token

        public string StepName { get; set; }

        public string? Note { get; set; }

        public int Status { get; set; }

        public DateOnly? PlanDate { get; set; }

        public DateOnly? DoneDate { get; set; }

        public string? DrugName { get; set; }

        public string? Dosage { get; set; } //Lieu luong
    }
}
