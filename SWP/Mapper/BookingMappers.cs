using SWP.Dtos.Booking;
using SWP.Models;

namespace SWP.Mapper
{
    public static class BookingMappers
    {
        public static BookingResponseDto ToBookingResponseDto(this Booking booking)
        {
            return new BookingResponseDto
            {

                Status = booking.StatusNavigation?.StatusName,
                DoctorName = booking.Doc?.Acc?.FullName,
                WorkDate = booking.Ds?.WorkDate ?? default,
                SlotStart = booking.Ds?.Slot?.SlotStart ?? default,
                SlotEnd = booking.Ds?.Slot?.SlotEnd ?? default,
                Note = booking.Note // ✅ Cho phép null, không cần ?? string.Empty
            };
        }
    }
}
