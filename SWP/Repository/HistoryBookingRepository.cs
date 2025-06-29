using Microsoft.EntityFrameworkCore;
using SWP.Dtos.Customer;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;

namespace SWP.Repository
{
    public class HistoryBookingRepository : IHistoryBookingRepository
    {
        private readonly HIEM_MUONContext _context;
        public HistoryBookingRepository(HIEM_MUONContext context)
        {
            _context = context;
        }
        public async Task<List<HistoryBookingDto>> GetHistoryBookingsAsync(int accId)
        {
            var hisBooking = await _context.Bookings
                .Where(b => b.AccId == accId)
                .Include(b => b.Doc)
                .Include(b => b.Ds)
                    .ThenInclude(ds => ds.Slot)
                .Include(b=> b.StatusNavigation)
                .ToListAsync();

            return hisBooking.Select(b => b.ToHistoryBookingDto()).ToList();
        }

    }
}
