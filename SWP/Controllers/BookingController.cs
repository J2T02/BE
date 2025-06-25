using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Data;
using SWP.Dtos.Booking;
using SWP.Dtos.Check;
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
        private readonly IUpdateBookingStatus _updateBookingStatusRepo;
        private readonly HIEM_MUONContext _context;

        public BookingController(IBookingRepository bookingRepository, HIEM_MUONContext context, IHistoryBookingRepository hisotryBookingRepository, IUpdateBookingStatus updateBookingStatus)
        {
            _bookingRepo = bookingRepository;
            _hisotryBookingRepository = hisotryBookingRepository;
            _context = context;
            _updateBookingStatusRepo = updateBookingStatus;
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
                        b.Status < 4).AnyAsync();


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

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBookingStatusRequestDto dto)
        {
            // 1. Tìm booking có id tương ứng

            var booking = await _updateBookingStatusRepo.UpdateBookingStatusAsync(id, dto);
            if (booking == null)
            {
                return NotFound(BaseRespone<UpdateBookingStatusRequestDto>.ErrorResponse(
                        "Không tìm thấy booking",
                        System.Net.HttpStatusCode.NotFound));
            }
            return Ok(BaseRespone<BookingDetailDto>.SuccessResponse(
                        booking.ToBookingDetail(),
                        "Cập nhật trạng thái đặt lịch thành công"));
        }

        [HttpGet("Check-Slot")]
        public async Task<IActionResult> CheckSlot([FromQuery] int? slotId, [FromQuery] DateOnly? fromDate, [FromQuery] DateOnly? toDate)
        {
            try
            {
                if (slotId <= 0 || fromDate > toDate)
                    return BadRequest("Nhập sai dữ liệu.");
                var request = new CheckSlotDoctorRequestDto
                {
                    SlotId = slotId,
                    FromDate = fromDate,
                    ToDate = toDate
                };
                var result = await _bookingRepo.CheckSlotDoctorAsync(request);
                return Ok(new BaseRespone<List<CheckSlotDoctorResponseDto>>(result, "Kiểm tra lịch thành công", HttpStatusCode.OK));
            }
            catch (Exception)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                     BaseRespone<CheckSlotDoctorResponseDto>.ErrorResponse(
                         "Kiểm tra lịch không thành công do lỗi hệ thống",
                         System.Net.HttpStatusCode.InternalServerError));
            }
        }
    }
}



