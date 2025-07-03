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
using SWP.Models.Vnpay;
using SWP.Repository;
using SWP.Service.Vnpay;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IHistoryBookingRepository _hisotryBookingRepository;
        private readonly IUpdateBookingStatus _updateBookingStatusRepo;
        private readonly IVnPayService _vn;
        private readonly HIEM_MUONContext _context;

        public BookingController(IBookingRepository bookingRepository, HIEM_MUONContext context, IHistoryBookingRepository hisotryBookingRepository, IUpdateBookingStatus updateBookingStatus, IVnPayService vn)
        {
            _bookingRepo = bookingRepository;
            _hisotryBookingRepository = hisotryBookingRepository;
            _context = context;
            _updateBookingStatusRepo = updateBookingStatus;
            _vn = vn;
        }
        [Authorize(Roles = "Customer")]
        [HttpPost("Booking")]
        public async Task<IActionResult> BookingAndPayAsync([FromBody] BookingRequestDto bookingRequest)
        {
            try
            {
                // ➊ Lấy accId từ JWT
                var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (accountIdClaim == null)
                {
                    return BadRequest(new BaseRespone<List<HistoryBookingDto>>(HttpStatusCode.BadRequest, "Không tìm thấy thông tin khách hàng"));
                }

                int accId = int.Parse(accountIdClaim);
                var today = DateTime.Today;

                // ➋ Kiểm tra khách đã có lịch pending/chưa thanh toán?
                bool hasPending = await _context.Bookings
                    .AnyAsync(b => b.AccId == accId && b.Status < 4);   // 1..3 = chưa khám

                if (hasPending)
                    return Conflict(BaseRespone<string>.ErrorResponse("Khách hàng đã đặt lịch, vui lòng đến khám", HttpStatusCode.Conflict));

                // ➌ Tạo booking (Status = 1 – Pending)
                var booking = await _bookingRepo.BookingAsync(bookingRequest, accId);
                if (booking == null)
                    return NotFound(BaseRespone<string>.ErrorResponse("Lịch đã đầy, chọn khung giờ khác", HttpStatusCode.NotFound));

                // ➍ Sinh URL VNPay
                var payInfo = new PaymentInformationModel
                {
                    OrderId = booking.BookingId,                  // bookingId gửi sang VNPay
                    Amount = 10000,                            // hoặc tính động
                    OrderDescription = $"Thanh toán đặt lịch {booking.BookingId}"
                };
                string paymentUrl = _vn.CreatePaymentUrl(payInfo, HttpContext);

                // ➎ Trả bookingId + paymentUrl để FE redirect
                var dto = booking.ToBookingResponseDto();
                return Ok(BaseRespone<object>.SuccessResponse(
                    new { booking = dto, paymentUrl },
                    "Tạo lịch thành công – chuyển sang VNPay"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse(ex.Message, HttpStatusCode.BadRequest));
            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    BaseRespone<string>.ErrorResponse($"Lỗi hệ thống: {ex.Message}", HttpStatusCode.InternalServerError));
            }
        }


        [HttpGet("History/{id}")]
        public async Task<IActionResult> GetHistoryBookings(int id)
        {
            try
            {
                var historyBookings = await _hisotryBookingRepository.GetHistoryBookingsAsync(id);
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
                        booking.ToBookingDetailDto(),
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
                if (result == null || result.Count == 0)
                {
                    return NotFound(new BaseRespone<List<CheckSlotDoctorResponseDto>>(HttpStatusCode.NotFound, "Không tìm thấy lịch làm việc phù hợp"));
                }
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

        [HttpGet("Check-Slot/{docId}")]
        public async Task<IActionResult> CheckSlotByDoctorId(int docId)
        {
            try
            {
                if (docId <= 0)
                    return BadRequest("Nhập sai dữ liệu.");

                var result = await _bookingRepo.CheckSlotByDoctorId(docId);
                if (result == null || result.Count == 0)
                {
                    return NotFound(new BaseRespone<List<Booking>>(HttpStatusCode.NotFound, "Không tìm thấy lịch làm việc của bác sĩ này"));
                }

                
                
                return Ok(new BaseRespone<List<CheckSlotDoctorResponseDto>>(result, "Kiểm tra lịch thành công", HttpStatusCode.OK));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                     BaseRespone<List<Booking>>.ErrorResponse(
                         "Kiểm tra lịch không thành công do lỗi hệ thống",
                         System.Net.HttpStatusCode.InternalServerError));
            }
        }
        [HttpGet("GetAllBooking")]
        public async Task<IActionResult> GetAllBooking()
        {
            var listBooking = await _bookingRepo.GetAllBooking();
            var listBookingDto = listBooking.Select(x => x.ToBookingDetailDto()).ToList();
            if(listBookingDto == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Lấy danh sách đặt lịch thất bại", ""));
            }
            return Ok(BaseRespone<List<BookingDetailDto>>.SuccessResponse(listBookingDto, "Lấy danh sách lịch hẹn thành công"));
        }
    }
}



