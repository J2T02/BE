using Microsoft.EntityFrameworkCore;
using SWP.Dtos.Booking;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class UpdateBookingRepo : IUpdateBooking
    {
        private readonly HIEM_MUONContext _context;

        public UpdateBookingRepo(HIEM_MUONContext context)
        {
            _context = context;
        }

        public async Task<Booking?> ChangeScheduleAsync(int bookingId, DateOnly workDate, int slotId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Ds).ThenInclude(ds => ds.Slot)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null || booking.DocId == null || booking.Status == 5)
                return null;

            DateTime bookingTime = booking.Ds.WorkDate.Value.ToDateTime(booking.Ds.Slot.SlotStart.Value);
            if ((bookingTime - DateTime.Now).TotalHours < 48)
                throw new InvalidOperationException("Không thể đổi lịch vì còn dưới 48 tiếng.");

            var oldSchedule = booking.Ds;

            var newSchedule = await _context.DoctorSchedules
                .Where(ds => ds.WorkDate == workDate &&
                             ds.SlotId == slotId &&
                             ds.DocId == booking.DocId &&
                             ds.IsAvailable == true &&
                             ds.MaxBooking > 0)
                .FirstOrDefaultAsync();

            if (newSchedule == null)
                throw new InvalidOperationException("Không tìm thấy lịch mới cho bác sĩ này.");

            booking.DsId = newSchedule.DsId;

            if (oldSchedule != null)
                oldSchedule.MaxBooking = (oldSchedule.MaxBooking ?? 0) + 1;

            newSchedule.MaxBooking = (newSchedule.MaxBooking ?? 0) - 1;
            if (newSchedule.MaxBooking <= 0)
                newSchedule.IsAvailable = false;

            await _context.SaveChangesAsync();
            var fullBooking = await _context.Bookings
    .Include(b => b.Acc)
        .ThenInclude(acc => acc.Customers)
    .Include(b => b.Doc)
        .ThenInclude(doc => doc.Acc)
    .Include(b => b.Doc)
        .ThenInclude(doc => doc.Edu)
    .Include(b => b.Ds)
        .ThenInclude(ds => ds.Slot)
    .Include(b => b.StatusNavigation)
    .FirstOrDefaultAsync(b => b.BookingId == booking.BookingId);

            return fullBooking;
        }

        public async Task<Booking?> ChangeDoctorAsync(int bookingId, int newDoctorId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Ds).ThenInclude(ds => ds.Slot)
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null || booking.Ds == null || booking.Status == 5)
                return null;

            DateTime bookingTime = booking.Ds.WorkDate.Value.ToDateTime(booking.Ds.Slot.SlotStart.Value);
            if ((bookingTime - DateTime.Now).TotalHours < 48)
                throw new InvalidOperationException("Không thể đổi bác sĩ vì còn dưới 48 tiếng.");

            var oldSchedule = booking.Ds;

            var newSchedule = await _context.DoctorSchedules
                .Where(ds => ds.WorkDate == booking.Ds.WorkDate &&
                             ds.SlotId == booking.Ds.SlotId &&
                             ds.DocId == newDoctorId &&
                             ds.IsAvailable == true &&
                             ds.MaxBooking > 0)
                .FirstOrDefaultAsync();

            if (newSchedule == null)
                throw new InvalidOperationException("Bác sĩ mới không có lịch phù hợp.");

            booking.DocId = newDoctorId;
            booking.DsId = newSchedule.DsId;

            if (oldSchedule != null)
                oldSchedule.MaxBooking = (oldSchedule.MaxBooking ?? 0) + 1;

            newSchedule.MaxBooking = (newSchedule.MaxBooking ?? 0) - 1;
            if (newSchedule.MaxBooking <= 0)
                newSchedule.IsAvailable = false;

            await _context.SaveChangesAsync();
            var fullBooking = await _context.Bookings
    .Include(b => b.Acc)
        .ThenInclude(acc => acc.Customers)
    .Include(b => b.Doc)
        .ThenInclude(doc => doc.Acc)
    .Include(b => b.Doc)
        .ThenInclude(doc => doc.Edu)
    .Include(b => b.Ds)
        .ThenInclude(ds => ds.Slot)
    .Include(b => b.StatusNavigation)
    .FirstOrDefaultAsync(b => b.BookingId == booking.BookingId);

            return fullBooking;
        }
    }
}
