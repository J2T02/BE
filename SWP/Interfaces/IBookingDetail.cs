using SWP.Data;
using SWP.Dtos.Booking;
using SWP.Models;

namespace SWP.Interfaces
{
    public interface IBookingDetail
    {
        Task<Booking> GetBookingDetailAsync(int id);
    }
}
