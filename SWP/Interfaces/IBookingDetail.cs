using SWP.Data;
using SWP.Dtos.Booking;

namespace SWP.Interfaces
{
    public interface IBookingDetail
    {
        Task<BaseRespone<BookingDetailDto>> GetBookingDetailAsync(int id);
    }
}
