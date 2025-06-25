namespace SWP.Dtos.DoctorSchedule
{
    public class SlotScheduleDto
    {
        public int SlotId { get; set; }

        public TimeOnly SlotStart { get; set; }

        public TimeOnly SlotEnd { get; set; }
    }
}
