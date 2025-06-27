using SWP.Models;

namespace SWP.Interfaces
{
    public interface IUpdateBooking
    {
        Task<Booking?> ChangeScheduleAsync(int bookingId, DateOnly workDate, int slotId);
        Task<Booking?> ChangeDoctorAsync(int bookingId, int newDoctorId);
    }
}
