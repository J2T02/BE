using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Data;
using SWP.Dtos.Customer;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryBookingController : ControllerBase
    {
        private readonly IHistoryBookingRepository _hisotryBookingRepository;
        private readonly HIEM_MUONContext _context;
        public HistoryBookingController(HIEM_MUONContext context, IHistoryBookingRepository hisotryBookingRepository)
        {
            _hisotryBookingRepository = hisotryBookingRepository;
            _context = context;
        }

        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> GetHistoryBookings()
        {
            try
            {
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
