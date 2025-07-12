using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Data;
using SWP.Dtos.Booking;
using SWP.Libaries;
using SWP.Models;
using SWP.Models.Vnpay;
using SWP.Service.Vnpay;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly HIEM_MUONContext _context;

        public PaymentController(IVnPayService vnPayService, HIEM_MUONContext context)
        {

            _vnPayService = vnPayService;
            _context = context;

        }
        [HttpGet]
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }




        [HttpGet("VnPayReturn")]
        public async Task<IActionResult> VnPayReturn()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response.OrderId <= 0)
                return BadRequest("OrderId không hợp lệ hoặc không được gửi từ VNPay.");

            var booking = await _context.Bookings
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.BookingId == response.OrderId);

            if (booking == null)
                return NotFound($"Không tìm thấy booking với ID = {response.OrderId}");

            // ✅ Luôn tạo mới bản ghi dù thành công hay thất bại
            var existingPayment = await _context.Payments
                .FirstOrDefaultAsync(p => p.TransactionId == response.TransactionId);

            if (existingPayment != null)
            {
                var ok = existingPayment.StatusId == 2 ? "true" : "false";
                return Redirect($"https://localhost:5173/bookingdetail/{response.OrderId}?success={ok}");
            }

            var payment = new Payment
            {
                BookingId = booking.BookingId,
                Amount = response.Amount,
                MethodId = 1,
                PaymentTypeId = 1,
                StatusId = response.Success ? 2 : 1, // ✅ luôn ghi 1 nếu thất bại/hủy
                PaymentDate = DateTime.Now,
                TransactionId = response.TransactionId
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Redirect từ backend về:
            return Redirect($"http://localhost:5173/payment-result/{booking.BookingId}?success={(response.Success ? "true" : "false")}");


        }






        [HttpGet("vnpay-repayment/{id}")]
        public async Task<IActionResult> Repayment(int id)
        {
            // 🔍 Kiểm tra tồn tại booking
            var booking = await _context.Bookings
                .Include(b => b.Payments) // Để lấy thông tin thanh toán gần nhất
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
                return NotFound(BaseRespone<Payment>.ErrorResponse(
                        "Không tìm thấy booking",
                        System.Net.HttpStatusCode.NotFound));

            // 🛑 Kiểm tra nếu đã thanh toán thành công (StatusId = 2)
            var latestPayment = booking.Payments.OrderByDescending(p => p.PaymentDate).FirstOrDefault();
            if (latestPayment != null && latestPayment.StatusId == 2)
                return BadRequest(BaseRespone<string>.ErrorResponse("Đơn này đã được thanh toán", HttpStatusCode.BadRequest));

            // ✅ Tạo thông tin thanh toán lại
            var paymentInfo = new PaymentInformationModel
            {
                OrderId = booking.BookingId,
                Amount = 10000, // hoặc booking.Amount, dùng đúng field
                OrderDescription = $"Thanh toán lại cho lịch {booking.BookingId}"
            };

            var paymentUrl = _vnPayService.CreatePaymentUrl(paymentInfo, HttpContext);
            return Ok(new
            {
                BookingId = booking.BookingId,
                PaymentUrl = paymentUrl
            });
        }


    }

}
