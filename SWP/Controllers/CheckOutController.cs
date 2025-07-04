using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SWP.Dtos.Payment;
using SWP.Interfaces;
using SWP.Models;
using SWP.Service.Vnpay;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckOutController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IBookingRepository _repo;
        private readonly IConfiguration _cfg;
        private readonly HIEM_MUONContext _context;

        public CheckOutController(IVnPayService vnPayService, IBookingRepository repo, IConfiguration cfg, HIEM_MUONContext context)
        {
            _vnPayService = vnPayService;
            _repo = repo;
            _cfg = cfg;
            _context = context;
        }

        [HttpGet("vnpay-callback")]
        public async Task<IActionResult> VnPayReturn()
        {
            var result = _vnPayService.PaymentExecute(Request.Query);

            // ➊ Tìm booking
            var booking = await _context.Bookings
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BookingId == result.OrderId);

            if (booking == null)
                return NotFound($"Không tìm thấy booking #{result.OrderId}");

            /* -------------------------------------------------
             * ➋ Kiểm tra bản ghi Payment tạm (TransactionId = \"0\")
             * ------------------------------------------------- */
            var pendingPayment = await _context.Payments
                .FirstOrDefaultAsync(p => p.BookingId == result.OrderId &&
                                          p.TransactionId == "0");

            // VNPay thành công (ResponseCode == \"00\" & Success = true)
            bool isPaidSuccess = result.Success && result.VnPayResponseCode == "00";

            if (pendingPayment != null)
            {
                // ➜ Update bản ghi tạm
                pendingPayment.StatusId = isPaidSuccess ? 2 : 1;   // 2 = Thành công, 1 = Hủy/thất bại
                pendingPayment.TransactionId = result.TransactionId;    // Ghi TransactionId thật
                pendingPayment.Amount = result.Amount;
                pendingPayment.PaymentDate = DateTime.Now;
                pendingPayment.MethodId = 2;                       // VNPay
                pendingPayment.PaymentTypeId = 1;

                await _context.SaveChangesAsync();
                return Ok(result);                                      // hoặc redirect nếu muốn
            }

            /* -------------------------------------------------
             * ➌ Nếu KHÔNG có bản ghi tạm  →  Thêm mới
             * ------------------------------------------------- */
            var newPayment = new Payment
            {
                BookingId = booking.BookingId,
                Amount = result.Amount,
                PaymentDate = DateTime.Now,
                MethodId = 2,
                PaymentTypeId = 1,
                StatusId = isPaidSuccess ? 2 : 1,
                TransactionId = result.TransactionId
            };

            _context.Payments.Add(newPayment);
            await _context.SaveChangesAsync();

            return Ok(result);
        }

    }
}
