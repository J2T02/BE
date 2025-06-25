using SWP.Dtos.DoctorSchedule;

namespace SWP.Dtos.Doctor
{
    public class DoctorScheduleDto
    {
        public int DsId { get; set; }

        public DateOnly? WorkDate { get; set; }

        public SlotScheduleDto Slot { get; set; }

        public bool? IsAvailable { get; set; }

        public int? MaxBooking { get; set; }
    }
}
