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
                    .Include(b => b.Ds) // Include DoctorSchedule to access MaxBooking
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null) return null;

                booking.Status = 5;
                
                // Increment MaxBooking of the related DoctorSchedule
                if (booking.Ds != null && booking.Ds.MaxBooking.HasValue)
                {
                    booking.Ds.MaxBooking += 1;
                }

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
