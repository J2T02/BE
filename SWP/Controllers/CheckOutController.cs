using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            
            if (result.VnPayResponseCode == "00")
            {
                var paymentInfo = new PaymentDto
                {
                    BookingId = result.OrderId,
                    Amount = result.Amount,
                    PaymentDate = DateTime.Now,
                    MethodId = 1, // Assuming 1 is the ID for VNPay
                    PaymentTypeId = 1, // Assuming 1 is the ID for online payment
                    StatusId = 2, // Assuming 2 is the ID for successful payment
                    TransactionId = result.TransactionId,
                    TreatmentPlansId = null // Assuming no treatment plan is associated
                };
                _context.Payments.Add(new Payment
                {
                    BookingId = paymentInfo.BookingId,
                    Amount = paymentInfo.Amount,
                    PaymentDate = paymentInfo.PaymentDate,
                    MethodId = paymentInfo.MethodId,
                    PaymentTypeId = paymentInfo.PaymentTypeId,
                    StatusId = paymentInfo.StatusId,
                    TransactionId = paymentInfo.TransactionId
                });
                await _context.SaveChangesAsync();

                

            }else if(result.VnPayResponseCode == "24")
            {
                var paymentInfo = new PaymentDto
                {
                    BookingId = result.OrderId,
                    Amount = result.Amount,
                    PaymentDate = DateTime.Now,
                    MethodId = 1, // Assuming 1 is the ID for VNPay
                    PaymentTypeId = 1, // Assuming 1 is the ID for online payment
                    StatusId = 1, // Assuming 2 is the ID for successful payment
                    TransactionId = result.TransactionId,
                    TreatmentPlansId = null // Assuming no treatment plan is associated
                };
                _context.Payments.Add(new Payment
                {
                    BookingId = paymentInfo.BookingId,
                    Amount = paymentInfo.Amount,
                    PaymentDate = paymentInfo.PaymentDate,
                    MethodId = paymentInfo.MethodId,
                    PaymentTypeId = paymentInfo.PaymentTypeId,
                    StatusId = paymentInfo.StatusId,
                    TransactionId = paymentInfo.TransactionId
                });
                await _context.SaveChangesAsync();
            }

                // Fix: Replace 'View' with 'Ok' to return a valid JSON response.
                return Ok(result);
        }
    }
}
