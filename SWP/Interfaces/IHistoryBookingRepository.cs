using SWP.Dtos.Customer;

namespace SWP.Interfaces
{
    public interface IHistoryBookingRepository
    {
        Task<List<HistoryBookingDto>> GetHistoryBookingsAsync(int cusId);

    }
}
