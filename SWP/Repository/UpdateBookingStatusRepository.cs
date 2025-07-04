using Microsoft.EntityFrameworkCore;
using SWP.Dtos.Booking;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class UpdateBookingStatusRepository : IUpdateBookingStatus
    {
        private readonly HIEM_MUONContext _context;
        public UpdateBookingStatusRepository(HIEM_MUONContext context)
        {
            _context = context;
        }

        public async Task<Booking> UpdateBookingStatusAsync(int bookingId, UpdateBookingStatusRequestDto status)
        {
            var booking = await _context.Bookings
                .Include(b=> b.StatusNavigation)

                .Include(c => c.Acc)
                    .ThenInclude(x => x.Customers)
                .Include(d => d.Doc)
                .Include(d => d.Ds)
                    .ThenInclude(ds => ds.Slot)
                    
                .FirstOrDefaultAsync(x => x.BookingId == bookingId);
            if (booking == null)
            {
                return null;
            }

            booking.Status = status.Status;

            await _context.SaveChangesAsync();

            return booking;
        }
    }
}
