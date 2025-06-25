using Microsoft.EntityFrameworkCore;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class CancelBookingRepo
    {
        public class CancelBooking : ICancelBooking
        {
            private readonly HIEM_MUONContext _context;
            public CancelBooking(HIEM_MUONContext context)
            {
                _context = context;
            }
            public async Task<Booking?> CancelBookingAsync(int bookingId)
            {
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null) return null;

                booking.Status = 5;
                await _context.SaveChangesAsync();

                // 🔁 Nạp lại với StatusNavigation để chắc chắn không bị null
                booking = await _context.Bookings
                    .Include(b => b.StatusNavigation)
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                return booking;
            }
        }
    }
}
