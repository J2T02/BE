using SWP.Dtos.Booking;
using SWP.Dtos.Check;
using SWP.Models;

namespace SWP.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> BookingAsync(BookingRequestDto booking, int accId);
        Task<List<CheckSlotDoctorResponseDto>> CheckSlotDoctorAsync(CheckSlotDoctorRequestDto request);
        Task<List<CheckSlotDoctorResponseDto>> CheckSlotByDoctorId(int docId);
    }
}
