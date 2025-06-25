using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP.Data;
using SWP.Dtos.Booking;
using SWP.Interfaces;
using SWP.Repository;
using System.Net;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CancelBookingController : ControllerBase
    {
        private readonly ICancelBooking _cancelBookingRepo;

        public CancelBookingController(ICancelBooking cancelBookingRepo)
        {
            _cancelBookingRepo = cancelBookingRepo;
        }

        [HttpPut("cancel/{id}")]
        public async Task<BaseRespone<BookingStatusDto>> CancelBooking(int id)
        {
            try
            {
                var booking = await _cancelBookingRepo.CancelBookingAsync(id);

                if (booking == null)
                {
                    return new BaseRespone<BookingStatusDto>(
                        statusCode: HttpStatusCode.NotFound,
                        message: "Không tìm thấy lịch đặt"
                    );
                }

                var statusDto = new BookingStatusDto
                {
                    StatusId = booking.StatusNavigation.StatusId,
                    StatusName = booking.StatusNavigation.StatusName
                };

                return new BaseRespone<BookingStatusDto>(
                    data: statusDto,
                    message: "Hủy lịch đặt thành công",
                    statusCode: HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                return new BaseRespone<BookingStatusDto>(
                    statusCode: HttpStatusCode.InternalServerError,
                    message: "Lỗi hệ thống: " + ex.Message
                );
            }
        }


    }
}
