using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using SWP.Data;
using SWP.Dtos.Booking;
using SWP.Dtos.Check;
using SWP.Interfaces;
using SWP.Mapper;
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

        public async Task<Booking> BookingAsync(BookingRequestDto booking, int accId)
        {


            // 1. Lấy danh sách lịch làm việc phù hợp
            var slotSchedules = await _context.DoctorSchedules
                .Include(ds => ds.Bookings)
                .Where(ds => ds.WorkDate == booking.WorkDate && ds.SlotId == booking.SlotId && ds.IsAvailable == true && ds.MaxBooking > 0)
                .ToListAsync();

            // Kiểm tra xem còn lịch trống không (dựa trên SlotId, WorkDate)
            var hasAvailableSchedule = await _context.DoctorSchedules
                .AnyAsync(ds => ds.SlotId == booking.SlotId
                                && ds.WorkDate == booking.WorkDate
                                && ds.IsAvailable == true
                                && ds.MaxBooking > 0);

            if (!hasAvailableSchedule)
            {
                throw new InvalidOperationException("Lịch đã đầy. Vui lòng chọn thời gian khác.");
            }

            // 2. Chọn lịch làm việc
            DoctorSchedule? selectedSchedule = null;

            if (booking.DoctorId.HasValue)
            {
                // Nếu người dùng chọn bác sĩ
                selectedSchedule = slotSchedules
                .Where(ds => ds.DocId == booking.DoctorId && ds.MaxBooking > 0)
                .OrderByDescending(ds => ds.MaxBooking)
                .FirstOrDefault();
            }
            else
            {
                // Nếu không chọn bác sĩ, hệ thống chọn bác sĩ có ít booking nhất
                selectedSchedule = slotSchedules
                .Where(ds => ds.MaxBooking > 0)
                .OrderByDescending(ds => ds.MaxBooking)
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
                AccId = accId,
                DocId = selectedSchedule.DocId,
                DsId = selectedSchedule.DsId,
                Status = 1,
                CreateAt = DateTime.Now,
                Note = booking.Note
            };

            await _context.Bookings.AddAsync(newBooking);

            // giảm max booking của lịch làm việc
            selectedSchedule.MaxBooking--;

            //Nếu bằng 0 thì đánh dấu không còn trống
            if (selectedSchedule.MaxBooking == 0)
            {
                selectedSchedule.IsAvailable = false;
            }

            _context.DoctorSchedules.Update(selectedSchedule);
            await _context.SaveChangesAsync();

            // 6. Load lại booking để lấy navigation properties
            var fullBooking = await _context.Bookings
                .Include(b => b.Doc)
                    .ThenInclude(doc => doc.Acc)
                .Include(b => b.Ds)
                    .ThenInclude(ds => ds.Slot)
                .Include(b => b.StatusNavigation)
                .FirstOrDefaultAsync(b => b.BookingId == newBooking.BookingId);

            if (fullBooking == null)
            {
                throw new InvalidOperationException("Tạo booking thất bại.");
            }

            return fullBooking;
        }

        public async Task<List<CheckSlotDoctorResponseDto>> CheckSlotDoctorAsync(CheckSlotDoctorRequestDto request)
        {
            var query = _context.DoctorSchedules
                .Include(ds => ds.Doc)
                    .ThenInclude(doc => doc.Acc)
                .Where(ds =>
                        ds.IsAvailable == true &&
                        ds.DocId != null);

            if (request.SlotId.HasValue && request.SlotId.Value > 0)
            {
                query = query.Where(ds => ds.SlotId == request.SlotId.Value);
            }

            if (request.FromDate.HasValue)
            {
                query = query.Where(ds => ds.WorkDate >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(ds => ds.WorkDate <= request.ToDate.Value);
            }

            var checkSlot = await query
                .Select(ds => new CheckSlotDoctorResponseDto
                {
                    DocId = ds.DocId,
                    DoctorName = ds.Doc.Acc.FullName,
                    slotId = ds.SlotId.Value,
                    WorkDate = ds.WorkDate.Value,
                })
                .ToListAsync();
            return checkSlot;
        }

        public async Task<List<CheckSlotDoctorResponseDto>> CheckSlotByDoctorId(int docId)
        {
            var result = await _context.DoctorSchedules
        .Include(ds => ds.Doc)
            .ThenInclude(doc => doc.Acc)
        .Where(ds =>
            ds.DocId == docId &&
            ds.WorkDate.HasValue &&
            ds.SlotId.HasValue &&
            ds.IsAvailable == true
        )
        .Select(ds => new CheckSlotDoctorResponseDto
        {
            DocId = ds.DocId,
            DoctorName = ds.Doc.Acc.FullName,
            slotId = ds.SlotId ?? 0,
            WorkDate = ds.WorkDate ?? DateOnly.MinValue
        })
        .Distinct() // loại bỏ trùng nếu có nhiều bản ghi giống nhau
        .OrderByDescending(ds => ds.WorkDate) // Sắp xếp theo WorkDate giảm dần

        .ToListAsync();

            return result;
        }

        public async Task<List<Booking>> GetAllBooking()
        {
            var result = await _context.Bookings
                                                .Include(x => x.Doc)
                                                    .ThenInclude(d => d.Acc).ThenInclude(x=>x.Role)
                                                .Include(x=>x.Doc).ThenInclude(x => x.StatusNavigation)
                                                .Include(x => x.Doc).ThenInclude(x => x.Edu)
                                                .Include(x => x.Ds)
                                                    .ThenInclude(ds => ds.Slot)
                                                .Include(x => x.StatusNavigation)
                                                .ToListAsync();
            return result;


        }
        public async Task MarkAsPaid(int orderId)
        {
            var bk = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == orderId);
            if (bk == null) throw new Exception("Không tìm thấy booking");
            if (bk.Status == 2) return;                 // đã Paid thì bỏ qua
            bk.Status = 2;                              // 1 = Pending, 2 = Paid
            await _context.SaveChangesAsync();
        }
    }


}
