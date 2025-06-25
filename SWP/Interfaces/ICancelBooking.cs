using SWP.Models;

namespace SWP.Interfaces
{
    public interface ICancelBooking
    {
        Task<Booking?> CancelBookingAsync(int bookingId);
    }
}
