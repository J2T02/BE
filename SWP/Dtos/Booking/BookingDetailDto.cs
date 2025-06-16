using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Dtos.Payment;
using SWP.Dtos.Account;

namespace SWP.Dtos.Booking

{

    public class BookingDetailDto
    {
        public int BookingId { get; set; }
        public DateTime? CreateAt { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DocDto Doc { get; set; }
        public DocScheduleDto Schedule { get; set; }
    }
}
