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
        public async Task<BaseRespone<BookingDetailDto>> GetBookingDetailAsync(int id)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Cus)
                    .Include(b => b.Doc)
                    .Include(b => b.Ds)
                        .ThenInclude(ds => ds.Slot)
                    .Include(b => b.Ds)
                        .ThenInclude(ds => ds.Room)
                    .FirstOrDefaultAsync(b => b.BookingId == id);
                if (booking == null)
                {
                    return new BaseRespone<BookingDetailDto>(
                        statusCode: HttpStatusCode.NotFound,
                        message: "Không tìm thấy booking"
                    );
                }
                var bookingDto = booking.ToBookingDetailDto();
                return new BaseRespone<BookingDetailDto>(
                    data: bookingDto,
                    message: "Lấy dữ liệu thành công",
                    statusCode: HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                return new BaseRespone<BookingDetailDto>(
                    statusCode: HttpStatusCode.InternalServerError,
                    message: "Lỗi: " + ex.Message
                );
            }
        }
    }

}
