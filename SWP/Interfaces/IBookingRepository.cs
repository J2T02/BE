using SWP.Dtos.Booking;
using SWP.Models;

namespace SWP.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> BookingAsync(BookingRequestDto booking);
    }
}
