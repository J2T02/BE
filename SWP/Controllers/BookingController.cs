using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Data;
using SWP.Dtos.Booking;
using SWP.Dtos.Customer;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly HIEM_MUONContext _context;

        public BookingController(IBookingRepository bookingRepository, HIEM_MUONContext context)
        {
            _bookingRepo = bookingRepository;
            _context = context;
        }
        [Authorize(Roles = "Customer")]
        [HttpPost("Booking")]
        public async Task<IActionResult> BookingAsync([FromBody] BookingRequestDto bookingRequest)
        {
            try
            {
                var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (accountIdClaim == null)
                {
                    return BadRequest(new BaseRespone<List<HistoryBookingDto>>(HttpStatusCode.BadRequest, "Không tìm thấy thông tin khách hàng"));
                }

                int accountId = int.Parse(accountIdClaim);
                var today = DateTime.Today;

                
                var booked = await _context.Bookings.Where(b => b.AccId == accountId &&
                        b.Status <4).AnyAsync();


                if (booked)
                {
                    return Conflict(BaseRespone<BookingResponseDto>.ErrorResponse(
                        "Khách hàng đã đặt lịch vui lòng đến khám",
                        HttpStatusCode.Conflict));
                }

                
                var booking = await _bookingRepo.BookingAsync(bookingRequest, accountId);



                if (booking == null)
                {
                    return NotFound(BaseRespone<BookingResponseDto>.ErrorResponse(
                        "Lịch đã đầy vui lòng chọn thời gian khác",
                        System.Net.HttpStatusCode.NotFound));
                }
                var responseDto = booking.ToBookingResponseDto();

                return Ok(BaseRespone<BookingResponseDto>.SuccessResponse(responseDto, "Đặt lịch thành công"));


            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    BaseRespone<BookingResponseDto>.ErrorResponse(
                        "Đặt lịch không thành công do lỗi hệ thống",
                        System.Net.HttpStatusCode.InternalServerError));
            }
        }
    }
}



