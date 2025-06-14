using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetHistoryBookings(int Id)
        {
            try
            {
                var historyBookings = await _hisotryBookingRepository.GetHistoryBookingsAsync(Id);
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
