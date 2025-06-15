using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Data;
using SWP.Dtos.Booking;
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
        private readonly IBookingDetail _bookingDetailRepo;

        public BookingDetailController(IBookingDetail bookingDetailRepo)
        {
            _bookingDetailRepo = bookingDetailRepo;
        }

        [HttpGet("GetBookingDetail/{id}")]
        public async Task<BaseRespone<BookingDetailDto>> GetBookingDetail(int id)
        {
            return await _bookingDetailRepo.GetBookingDetailAsync(id);
        }
    }
}
