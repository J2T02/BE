using Microsoft.EntityFrameworkCore;
using SWP.Dtos.Doctor;
using SWP.Interfaces;
using SWP.Models;

namespace SWP.Repository
{
    public class DoctorRepository : IDoctor
    {
        private readonly HIEM_MUONContext _context;
        public DoctorRepository(HIEM_MUONContext context)
        {
            _context = context;
        }

        public async Task<Doctor> CreateDoctorAsync(Doctor newDoctor)
        {
            await _context.Doctors.AddAsync(newDoctor);
            await _context.SaveChangesAsync();
            return newDoctor;
        }

        public async Task<Doctor?> GetDoctorByIdAsync(int id)
        {
            return await _context.Doctors.Include(d => d.Acc).ThenInclude(a => a.Role)
                .Include(x => x.StatusNavigation)
                .Include(x => x.Edu)
                .Include(x => x.Certificates).FirstOrDefaultAsync(d => d.DocId == id);
        }


        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _context.Doctors.Include(d => d.Acc).ThenInclude(a => a.Role)
                .Include(x => x.StatusNavigation)
                .Include(x => x.Edu)
                .Include(x => x.Certificates).ToListAsync();
        }
        public async Task<Account> GetAccountByDoctor(Doctor doctor)
        {
            return await _context.Accounts.FirstOrDefaultAsync(x => x.AccId == doctor.Acc.AccId);
        }
        public async Task<Doctor> UpdateDoctorAsync(int id, UpdateDoctorRequestDto doctor)
        {
            var existDoctor = await GetDoctorByIdAsync(id);
            var doctorAccount = await GetAccountByDoctor(existDoctor);
            if (doctorAccount == null)
            {
                throw new Exception("Không tìm thấy tài khoản bác sĩ");
            }
            if(existDoctor == null)
            {
                throw new Exception("Không tìm thấy bác sĩ");
            }
            //existDoctor.DocName = doctor.DocName;
            existDoctor.Gender = doctor.Gender;
            existDoctor.Yob = doctor.Yob;
            existDoctor.Experience = doctor.Experience;
            existDoctor.EduId = doctor.Edu_Id;
            existDoctor.Status = doctor.Status;
          

            doctorAccount.Mail = doctor.Mail;
            doctorAccount.Phone = doctor.Phone;
            doctorAccount.FullName = doctor.FullName;
            doctorAccount.Img = doctor.Img;

            await _context.SaveChangesAsync();
            return existDoctor;
        }
        public async Task<string> DeleteDoctorAsync(int id)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(x => x.DocId == id);
            
            if (doctor == null)
            {
                return "Không tìm thấy bác sĩ";
            }
            var accountDoctor = await GetAccountByDoctor(doctor);
            accountDoctor.IsActive = false;
            if (accountDoctor == null)
            {
                return "Không tìm thấy tài khoản";
            }
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return accountDoctor.FullName;
        }

        public async Task<DoctorSchedule> GetDoctorScheduleByIdAsync(int DsId)
        {
            return await _context.DoctorSchedules.Include(ds => ds.Slot).Include(ds => ds.Doc).ThenInclude(doc => doc.Acc).FirstOrDefaultAsync(x => DsId == x.DsId);
        }

        public async Task<DoctorSchedule> RegisterDoctorSchedule(DoctorSchedule doctorSchedule)
        {
            var doctorModel = _context.DoctorSchedules.Add(doctorSchedule);
            await _context.SaveChangesAsync();
            return doctorModel.Entity;
        }

        public async Task<Doctor?> GetDoctorByAccountId(int accountId)
        {
            var doctorModel = await _context.Doctors
                .Include(d => d.Acc).ThenInclude(a => a.Role)
                .FirstOrDefaultAsync(x => x.AccId == accountId);
            return doctorModel;
        }

        public Task<List<DoctorSchedule>> GetDoctorScheduleIsTrue(int doctorId)
        {
            return _context.DoctorSchedules.Include(ds => ds.Slot).Include(ds => ds.Doc).ThenInclude(doc => doc.Acc).Where(ds => ds.IsAvailable == true && ds.DocId == doctorId).ToListAsync();
        }

        public Task<List<DoctorSchedule>> GetAllDoctorSchedule(int doctorId)
        {
            return _context.DoctorSchedules.Include(ds => ds.Slot).Include(ds => ds.Doc).ThenInclude(doc => doc.Acc).Where(x => x.DocId == doctorId).ToListAsync();
        }

        public async Task<SlotSchedule?> GetSlotById(int id)
        {
            return await _context.SlotSchedules.FindAsync(id);
        }

        public Task<List<DoctorSchedule>?> GetDoctorScheduleByDate(DateOnly request)
        {
            return _context.DoctorSchedules
                .Include(ds => ds.Slot).Include(ds => ds.Doc).ThenInclude(doc => doc.Acc)
                .Where(x => x.WorkDate == request && x.IsAvailable == true).ToListAsync();
        }

        public Task<Doctor> GetDoctorByAccountIdAsync(int accountId)
        {
            return _context.Doctors
                .Include(d => d.Acc)
                .ThenInclude(a => a.Role)
                .FirstOrDefaultAsync(d => d.AccId == accountId);
        }

        public Task<DoctorStatus> GetDoctorStatusById(int id)
        {
            return _context.DoctorStatuses.FirstOrDefaultAsync(x => x.StatusId == id);
        }

        public async Task<Doctor> UpdateDoctorStatus(int id, UpdateDoctorStatusDto request)
        {
            var exist = await GetDoctorByIdAsync(id);
            if (exist == null)
            {
                throw new Exception("Không tìm thấy bác sĩ");
            }
            exist.Status = request.StatusId;
            _context.Doctors.Update(exist);
            await _context.SaveChangesAsync();
            return exist;
        }
    }
}
