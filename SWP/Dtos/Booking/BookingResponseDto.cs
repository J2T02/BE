namespace SWP.Dtos.Booking
{
    public class BookingResponseDto
    {
        
        public int BookingId { get; set; }
        public string Status { get; set; } 

        public string DoctorName { get; set; } = string.Empty;

        public DateOnly WorkDate { get; set; }

        public TimeOnly SlotStart { get; set; }

        public TimeOnly SlotEnd { get; set; }

        public string? Note { get; set; }

    }
}
