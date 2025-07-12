namespace SWP.Dtos.StepDetail
{
    public class CreateStepDetailDto
    {
        //public int SdId { get; set; }

        public int TpId { get; set; }

        public int TsId { get; set; }

        public int DocId { get; set; }

        public string StepName { get; set; }

        public string? Note { get; set; }

        //public int Status { get; set; } mac dinh la 1

        public int DsId { get; set; }

        //public DateOnly? DoneDate { get; set; } ngay thanh cong se update sau khi hoan thanh

        public string? DrugName { get; set; }

        public string? Dosage { get; set; } //Lieu luong
    }
}
