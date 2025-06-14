using SWP.Dtos.Booking;
using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Models;

namespace SWP.Mapper
{
    public static class BookingMapper
    {
        public static BookingDetailDto ToBookingDetailDto(this Booking booking)
        {
            return new BookingDetailDto
            {
                BookingId = booking.BookingId,
                CreateAt = booking.CreateAt,
                Status = booking.Status,
                Note = booking.Note,

                Cus = new CustomerDto
                {
                    HusName = booking.Cus?.HusName,
                    WifeName = booking.Cus?.WifeName,
                    Phone = booking.Cus?.Phone,
                    Mail = booking.Cus?.Mail
                },

                Doc = new DoctorDto
                {
                    DocName = booking.Doc?.DocName,
                    Phone = booking.Doc?.Phone,
                    Mail = booking.Doc?.Mail,
                    //Specialized = booking.Doc?.Specialized
                },

                Schedule = new DocScheduleDto
                {
                    Date = booking.Ds?.WorkDate,
                    Start = booking.Ds?.Slot?.SlotStart,
                    End = booking.Ds?.Slot?.SlotEnd,
                    Room = booking.Ds?.Room?.RoomNumber
                }
            };
        }
    }
}
