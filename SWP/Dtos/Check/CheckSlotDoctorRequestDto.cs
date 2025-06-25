namespace SWP.Dtos.Check
{
    public class CheckSlotDoctorRequestDto
    {
        public int? SlotId { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
    }
}
