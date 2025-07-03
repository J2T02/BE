using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Dtos.Payment;
using SWP.Dtos.Account;
using SWP.Dtos.DoctorSchedule;

namespace SWP.Dtos.Booking

{

    public class BookingDetailDto
    {
        public int BookingId { get; set; }
        public DateTime? CreateAt { get; set; }
        //public int Status { get; set; }
        public BookingStatusDto Status { get; set; }
        public string Note { get; set; }
        public CustomerDto Cus { get; set; }
        public DocDto Doc { get; set; }
        public DocScheduleDto Schedule { get; set; }
        public SlotScheduleDto Slot { get; set; }
    

    }
}
