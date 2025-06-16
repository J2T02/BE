using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Data;
using SWP.Dtos.Booking;
using SWP.Dtos.Doctor;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
using System.Net;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingDetailController : ControllerBase
    {
        private readonly HIEM_MUONContext _context;
        private readonly IBookingDetail _bookingDetailRepo;

        public BookingDetailController(HIEM_MUONContext context,IBookingDetail bookingDetail)
        {
            _context = context;
            _bookingDetailRepo = bookingDetail;
        }

        [HttpGet("{id}")]
        public async Task<BaseRespone<BookingDetailDto>> GetBookingDetail(int id)
        {
            try
            {
                var booking = await _bookingDetailRepo.GetBookingDetailAsync(id);
                
                if (booking == null)
                {
                    return new BaseRespone<BookingDetailDto>(
                        HttpStatusCode.NotFound,
                        "Không tìm thấy thông tin đặt lịch"
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
