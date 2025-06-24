using SWP.Dtos.Booking;
using SWP.Models;

namespace SWP.Interfaces
{
    public interface IUpdateBookingStatus
    {
        Task<Booking> UpdateBookingStatusAsync(int bookingId, UpdateBookingStatusRequestDto status);
    }
}
