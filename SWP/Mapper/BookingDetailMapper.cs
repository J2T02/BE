using SWP.Dtos.Booking;
using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Models;
using SWP.Dtos.Account;

namespace SWP.Mapper
{
    public static class BookingDetailMapper
    {
        public static BookingDetailDto ToBookingDetailDto(this Booking booking)
        {
            return new BookingDetailDto
            {
                BookingId = booking.BookingId,
                CreateAt = booking.CreateAt,
                Status = booking.Status,
                Note = booking.Note,

                Doc = new DoctorDto
                {
                    DocName = booking.Doc?.DocName,
                    Phone = booking.Doc?.Phone,
                    Mail = booking.Doc?.Mail,
                    //Gender = booking.Doc?.Gender,
                    //Yob = booking.Doc?.Yob,
                    Certification = booking.Doc?.Certification,
                    Experience = booking.Doc?.Experience,
                    //Specialized = booking.Doc?.Specialized
                },

                Schedule = new DoctorScheduleDto
                {
                    //DocName = booking.Doc?.DocName,
                    WorkDate = booking.Ds?.WorkDate,
                    SlotId = booking.Ds?.SlotId,
                    //IsAvailable = booking.Ds?.IsAvailable,
                    //Room = booking.Ds?.Room?.RoomNumber
                }
            };
        }
    }
}
