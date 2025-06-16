namespace SWP.Dtos.Booking
{
    public class BookingResponseDto
    {
        

        public string Status { get; set; } = string.Empty;

        public string DoctorName { get; set; } = string.Empty;

        public DateOnly WorkDate { get; set; }

        public TimeOnly SlotStart { get; set; }

        public TimeOnly SlotEnd { get; set; }

        public string? Note { get; set; }

    }
}
