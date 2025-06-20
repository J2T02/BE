using System.Linq;
using Microsoft.EntityFrameworkCore;
using SWP.Dtos.Booking;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly HIEM_MUONContext _context;

        private const int MAX_PATIENTS_PER_SLOT = 10;

        public BookingRepository(HIEM_MUONContext context)
        {
            _context = context;
        }

        public async Task<Booking> BookingAsync(BookingRequestDto booking)
        {
            const int MAX_PATIENTS_PER_SLOT = 5;

            // 1. Lấy danh sách lịch làm việc phù hợp
            var slotSchedules = await _context.DoctorSchedules
                .Include(ds => ds.Bookings)
                .Where(ds => ds.WorkDate == booking.WorkDate && ds.SlotId == booking.SlotId && ds.IsAvailable == true )
                .ToListAsync();

            // 2. Chọn lịch làm việc
            DoctorSchedule? selectedSchedule = null;

            if (booking.DoctorId.HasValue)
            {
                // Nếu người dùng chọn bác sĩ
                selectedSchedule = slotSchedules
                    .FirstOrDefault(ds => ds.DocId == booking.DoctorId && ds.Bookings.Count < MAX_PATIENTS_PER_SLOT);
            }
            else
            {
                // Nếu không chọn bác sĩ, hệ thống chọn bác sĩ có ít booking nhất
                selectedSchedule = slotSchedules
                    .Where(ds => ds.Bookings.Count < MAX_PATIENTS_PER_SLOT)
                    .OrderBy(ds => ds.Bookings.Count)
                    .FirstOrDefault();
            }

            // 3. Nếu không có lịch trống thì throw exception
            if (selectedSchedule == null)
            {
                throw new InvalidOperationException("Không có lịch booking.");
            }

            // 4. Tạo và lưu booking
            var newBooking = new Booking
            {
                CusId = booking.CustomerId,
                DocId = selectedSchedule.DocId,
                DsId = selectedSchedule.DsId,
                Status = 1,
                CreateAt = DateTime.Now,
                Note = booking.Note
            };

            await _context.Bookings.AddAsync(newBooking);
            await _context.SaveChangesAsync();

            // 5. Cập nhật trạng thái slot nếu đã đầy
            selectedSchedule = await _context.DoctorSchedules
                .Include(ds => ds.Bookings)
                .FirstOrDefaultAsync(ds => ds.DsId == selectedSchedule.DsId);

            if (selectedSchedule != null && selectedSchedule.Bookings.Count >= MAX_PATIENTS_PER_SLOT)
            {
                selectedSchedule.IsAvailable = false;
                _context.DoctorSchedules.Update(selectedSchedule);
                await _context.SaveChangesAsync();
            }

            // 6. Load lại booking để lấy navigation properties
            var fullBooking = await _context.Bookings
                .Include(b => b.Doc)
                .Include(b => b.Ds)
                    .ThenInclude(ds => ds.Slot)
                .FirstOrDefaultAsync(b => b.BookingId == newBooking.BookingId);

            if (fullBooking == null)
            {
                throw new InvalidOperationException("Tạo booking thất bại.");
            }

            return fullBooking;
        }
    }
}
