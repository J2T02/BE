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
using SWP.Repository;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IHistoryBookingRepository _hisotryBookingRepository;
        private readonly HIEM_MUONContext _context;

        public BookingController(IBookingRepository bookingRepository, HIEM_MUONContext context, IHistoryBookingRepository hisotryBookingRepository)
        {
            _bookingRepo = bookingRepository;
            _hisotryBookingRepository = hisotryBookingRepository;
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseRespone<BookingResponseDto>.ErrorResponse(
                    ex.Message,
                    System.Net.HttpStatusCode.BadRequest));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    BaseRespone<BookingResponseDto>.ErrorResponse(
                        "Đặt lịch không thành công do lỗi hệ thống",
                        System.Net.HttpStatusCode.InternalServerError));
            }
        }

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> GetHistoryBookings()
        {
            try
            {
                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    return Unauthorized(new BaseRespone<List<HistoryBookingDto>>(HttpStatusCode.Unauthorized, "Chưa đăng nhập"));
                }
                var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (accountIdClaim == null)
                {
                    return BadRequest(new BaseRespone<List<HistoryBookingDto>>(HttpStatusCode.BadRequest, "Không tìm thấy thông tin khách hàng"));
                }

                int accountId = int.Parse(accountIdClaim);

               





                var historyBookings = await _hisotryBookingRepository.GetHistoryBookingsAsync(accountId);
                if (historyBookings == null || historyBookings.Count == 0)
                {
                    return NotFound(new BaseRespone<HistoryBookingDto>(HttpStatusCode.NotFound, "Không tìm thấy lịch sử đặt lịch cho khách hàng này"));
                }

                return Ok(new BaseRespone<List<HistoryBookingDto>>(historyBookings, "Lấy lịch sử đặt lịch thành công", HttpStatusCode.OK));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseRespone<List<HistoryBookingDto>>(HttpStatusCode.InternalServerError, $"Lỗi hệ thống: {ex.Message}"));
            }
        }
    }
}



