namespace SWP.Dtos.Booking
{
    public class BookingRequestDto
    {
        public int CustomerId { get; set; }

        

        public int SlotId { get; set; }

        public DateOnly WorkDate { get; set; }

        public int? DoctorId { get; set; }

        public string? Note { get; set; }
    }
}
