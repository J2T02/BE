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
                    .ThenInclude(acc => acc.Customers) // ✅ Thêm dòng này
                .Include(b => b.Doc)
                    .ThenInclude(doc => doc.Acc)       // nếu cần thông tin account bác sĩ
                .Include(b => b.Doc)
                    .ThenInclude(doc => doc.Edu)       // nếu dùng EducationLevel
                .Include(b => b.Ds)
                    .ThenInclude(ds => ds.Slot)
                .Include(b => b.StatusNavigation)      // ✅ nếu cần StatusName
                .FirstOrDefaultAsync(b => b.BookingId == id);

            return booking;
        }

        

    }

}
