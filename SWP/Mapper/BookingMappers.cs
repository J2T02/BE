﻿using SWP.Dtos.Booking;
using SWP.Dtos.Check;
using SWP.Models;

namespace SWP.Mapper
{
    public static class BookingMappers
    {
        public static BookingResponseDto ToBookingResponseDto(this Booking booking)
        {
            return new BookingResponseDto
            {
                BookingId = booking.BookingId,
                Status = booking.StatusNavigation?.StatusName,
                DoctorName = booking.Doc?.Acc?.FullName,
                WorkDate = booking.Ds?.WorkDate ?? default,
                SlotStart = booking.Ds?.Slot?.SlotStart ?? default,
                SlotEnd = booking.Ds?.Slot?.SlotEnd ?? default,
                Note = booking.Note // ✅ Cho phép null, không cần ?? string.Empty
            };
        }

        public static CheckSlotDoctorResponseDto ToCheckSlotResposeDto(this DoctorSchedule doctorSchedule)
        {
            return new CheckSlotDoctorResponseDto
            {
                DocId = doctorSchedule.Doc.DocId,
                DoctorName = doctorSchedule.Doc.Acc.FullName // Lấy tên bác sĩ từ tài khoản của bác sĩ
            };
        }
    }
}
