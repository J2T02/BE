using SWP.Dtos.Customer;
using SWP.Models;

namespace SWP.Mapper
{
    public static class HistoryBookingMapper
    {
        public static HistoryBookingDto ToHistoryBookingDto(this Booking historyBooking)
        {
            return new HistoryBookingDto
            {
                BookingId = historyBooking.BookingId,

                ScheduleInfo = historyBooking.Ds != null ? $"{historyBooking.Ds.WorkDate:dd/MM/yyyy}" : "chưa có lịch",
                Status = (int)historyBooking.Status,

            };
        }

    }
}
