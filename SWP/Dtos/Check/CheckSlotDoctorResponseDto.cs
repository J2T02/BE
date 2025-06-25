namespace SWP.Dtos.Check
{
    public class CheckSlotDoctorResponseDto
    {
        public int DocId { get; set; }
        public string DoctorName { get; set; }
        public int slotId { get; set; }
        public DateOnly WorkDate { get; set; }
    }
}
