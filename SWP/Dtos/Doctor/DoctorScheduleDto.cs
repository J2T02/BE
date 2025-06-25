namespace SWP.Dtos.Doctor
{
    public class DoctorScheduleDto
    {
        public int DsId { get; set; }

        public int? DocId { get; set; }

        public DateOnly? WorkDate { get; set; }

        public int? SlotId { get; set; }

        public bool? IsAvailable { get; set; }

        public int? MaxBooking { get; set; }
    }
}
