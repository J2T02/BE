using SWP.Dtos.Booking;
using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Models;
using SWP.Dtos.Account;

namespace SWP.Mapper
{
    public static class BookingDetailMapper
    {
        public static BookingDetailDto ToBookingDetail(this Booking booking)
        {
            return new BookingDetailDto
            {
                BookingId = booking.BookingId,
                CreateAt = booking.CreateAt,
                Status = 1,
                Note = booking.Note,

                Doc = new DocDto
                {
                    //DocName = booking.Doc?.DocName,
                    //Phone = booking.Doc?.Phone,
                    //Mail = booking.Doc?.Mail,
                    //Gender = booking.Doc?.Gender,
                    //Yob = booking.Doc?.Yob,
                    //Certification = booking.Doc?.Certification,
                    //Experience = booking.Doc?.Experience,
                },

                Schedule = new DocScheduleDto
                {
                    //DocName = booking.Doc?.DocName,
                    WorkDate = booking.Ds?.WorkDate,
                    SlotId = booking.Ds?.SlotId,
                    //IsAvailable = booking.Ds?.IsAvailable,

                }
            };
        }
        public static BookingDetailDto ToBookingDetailDto(this Booking booking)
        {
            return new BookingDetailDto
            {
                BookingId = booking.BookingId,
                CreateAt = booking.CreateAt,
                Status = 1,
                Note = booking.Note,

                Doc = new DocDto
                {
                    //DocName = booking.Doc?.DocName,
                    //Phone = booking.Doc?.Phone,
                    //Mail = booking.Doc?.Mail,
                    //Gender = booking.Doc?.Gender,
                    //Yob = booking.Doc?.Yob,
                    //Experience = booking.Doc?.Experience,
                    //Certification = booking.Doc?.Certification
                    // Thêm các trường khác nếu DoctorDto có
                },

                Schedule = new DocScheduleDto
                {
                    //DocName = booking.Doc?.DocName,
                    WorkDate = booking.Ds?.WorkDate,
                    SlotId = booking.Ds?.SlotId,
                    //IsAvailable = booking.Ds?.IsAvailable
                    // Thêm các trường khác nếu DoctorScheduleDto có
                }
            };
        }

    }
}
