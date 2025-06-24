using Microsoft.EntityFrameworkCore;
using SWP.Data;
using SWP.Dtos.Booking;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
using System.Net;

namespace SWP.Repository
{
    public class BookingDetailRepository : IBookingDetail
    {
        private readonly HIEM_MUONContext _context;
        public BookingDetailRepository(HIEM_MUONContext context)
        {
            _context = context;
        }
        public async Task<Booking?> GetBookingDetailAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Acc)
                .Include(b => b.Doc)
                .Include(b => b.Ds)
                    .ThenInclude(ds => ds.Slot)
                
                .FirstOrDefaultAsync(b => b.BookingId == id);

            return booking; // Trả về null nếu không tìm thấy
        }
    }

}
