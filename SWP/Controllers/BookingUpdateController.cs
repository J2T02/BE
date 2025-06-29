using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP.Data;
using SWP.Dtos.Booking;
using SWP.Interfaces;
using SWP.Mapper;
using System.Net;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingUpdateController : ControllerBase
    {
        private readonly IUpdateBooking _updateBookingRepo;

        public BookingUpdateController(IUpdateBooking updateBookingRepo)
        {
            _updateBookingRepo = updateBookingRepo;
        }

        [HttpPut("update-schedule/{id}")]
        public async Task<BaseRespone<BookingDetailDto>> UpdateSchedule(int id, [FromBody] UpdateBookingScheduleDto dto)
        {
            try
            {
                var updatedBooking = await _updateBookingRepo.ChangeScheduleAsync(id, dto.WorkDate, dto.SlotId);
                if (updatedBooking == null)
                    return new BaseRespone<BookingDetailDto>(
                        HttpStatusCode.NotFound,
                        "Không tìm thấy booking hoặc lịch bị hủy"
                    );

                return new BaseRespone<BookingDetailDto>(
                    data: updatedBooking.ToBookingDetailDto(),
                    message: "Cập nhật ngày khám thành công",
                    statusCode: HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                return new BaseRespone<BookingDetailDto>(
                    statusCode: HttpStatusCode.BadRequest,
                    message: "Lỗi: " + ex.Message
                );
            }
        }

        [HttpPut("update-doctor/{id}")]
        public async Task<BaseRespone<BookingDetailDto>> UpdateDoctor(int id, [FromBody] UpdateBookingDoctorDto dto)
        {
            try
            {
                var updatedBooking = await _updateBookingRepo.ChangeDoctorAsync(id, dto.DoctorId);
                if (updatedBooking == null)
                    return new BaseRespone<BookingDetailDto>(
                        HttpStatusCode.NotFound,
                        "Không tìm thấy booking hoặc lịch bị hủy"
                    );

                return new BaseRespone<BookingDetailDto>(
                    data: updatedBooking.ToBookingDetailDto(),
                    message: "Cập nhật bác sĩ thành công",
                    statusCode: HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                return new BaseRespone<BookingDetailDto>(
                    statusCode: HttpStatusCode.BadRequest,
                    message: "Lỗi: " + ex.Message
                );
            }
        }
    }
}
