using SWP.Dtos.Booking;
using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Models;
using SWP.Dtos.Account;
using SWP.Dtos.DoctorSchedule;

namespace SWP.Mapper
{
    public static class BookingDetailMapper
    {
        public static BookingDetailDto ToBookingDetailDto(this Booking booking)
        {
            var customer = booking.Acc?.Customers?.FirstOrDefault();
            return new BookingDetailDto
            {
                BookingId = booking.BookingId,
                CreateAt = booking.CreateAt,
                Status = new BookingStatusDto
                {
                    StatusId = booking.Status ?? 0,
                    StatusName = booking.StatusNavigation?.StatusName ?? "Trạng thái không tồn tại"
                },
                Note = booking.Note,

                Cus = customer != null
                    ? new CustomerDto
                {
                    CusId = customer.CusId,
                    HusName = customer.HusName,
                    HusYob = customer.HusYob,
                    WifeName = customer.WifeName,
                    WifeYob = customer.WifeYob,
                    AccCus = new AccountDetailResponeDto
                    {
                        AccId = booking.Acc?.AccId ?? 0,
                        FullName = booking.Acc?.FullName,
                        Phone = booking.Acc?.Phone,
                        Mail = booking.Acc?.Mail
                    }
                }
                : null, // hoặc new CustomerDto() nếu bạn muốn object rỗng thay vì null


                Doc = new DocDto
                {
                    DocId = booking?.Doc?.DocId.ToString(),
                    AccDoc = new AccountDetailResponeDto
                    {
                        AccId = booking.Doc?.Acc?.AccId ?? 0,
                        FullName = booking.Doc?.Acc?.FullName,
                        Phone = booking.Doc?.Acc?.Phone,
                        Mail = booking.Doc?.Acc?.Mail,
                    }
                },
                Schedule = new DocScheduleDto
                {
                    WorkDate = booking.Ds?.WorkDate,
                    SlotId = booking.Ds?.SlotId,
                },

                Slot = booking.Ds.Slot != null
                    ? new SlotScheduleDto
                    {
                        SlotId = booking.Ds.Slot.SlotId,
                        SlotStart = booking.Ds.Slot.SlotStart ?? default(TimeOnly),
                        SlotEnd = booking.Ds.Slot.SlotEnd ?? default(TimeOnly),
                    }
                : null,

            };
        }
        //public static BookingDetailDto ToBookingDetailDtos(this Booking booking)
        //{
        //    return new BookingDetailDto
        //    {
        //        BookingId = booking.BookingId,
        //        CreateAt = booking.CreateAt,
        //        Status = new BookingStatusDto
        //        {
        //            StatusId = booking.Status ?? 0,
        //            StatusName = booking.StatusNavigation?.StatusName ?? "Unknown"
        //        },
        //        Note = booking.Note,
        //        Cus = new CustomerDto
        //        {
        //            HusName = booking.Acc?.Customers?.FirstOrDefault()?.HusName,
        //            HusYob = booking.Acc?.Customers?.FirstOrDefault()?.HusYob,
        //            WifeName = booking.Acc?.Customers?.FirstOrDefault()?.WifeName,
        //            WifeYob = booking.Acc?.Customers?.FirstOrDefault()?.WifeYob,
        //        },
        //        Doc = new DocDto
        //        {
        //            Gender = booking.Doc?.Gender,
        //            Yob = booking.Doc?.Yob,
        //            EducationLevel = booking.Doc?.Edu?.EduName,
        //            Experience = booking.Doc?.Experience,
        //            AccDoc = new AccountDetailResponeDto
        //            {
        //                FullName = booking.Doc?.Acc?.FullName,
        //                Phone = booking.Doc?.Acc?.Phone,
        //                Mail = booking.Doc?.Acc?.Mail,

        //            }
        //        },

        //        Schedule = new DocScheduleDto
        //        {
        //            WorkDate = booking.Ds?.WorkDate,
        //            SlotId = booking.Ds?.SlotId,
        //        }
        //    };
        //}

    }
}
