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
            return await _context.Doctors.Include(d => d.Acc).ThenInclude(a => a.Role).FirstOrDefaultAsync(d => d.DocId == id);
        }


        public async Task<List<Doctor>> GetAllDoctorsAsync()
        {
            return await _context.Doctors.Include(d => d.Acc).ThenInclude(a => a.Role).ToListAsync();
        }

        public async Task<Doctor> UpdateDoctorAsync(int id, UpdateDoctorRequestDto doctor)
        {
            var existDoctor = await _context.Doctors.FirstOrDefaultAsync(x => x.DocId == id);
            if(existDoctor == null)
            {
                throw new Exception("Doctor not found");
            }
            existDoctor.DocName = doctor.DocName;
            existDoctor.Gender = doctor.Gender;
            existDoctor.Yob = doctor.Yob;
            
            
            existDoctor.Experience = doctor.Experience;
            
            await _context.SaveChangesAsync();
            return existDoctor;
        }

        public async Task<string> DeleteDoctorAsync(int id)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(x => x.DocId == id);
            var docName = doctor.DocName;
            if(doctor == null)
            {
                return null;
            }
             _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return docName;

        }

        public async Task<DoctorSchedule> GetDoctorScheduleByIdAsync(DoctorSchedule doctorSchedule)
        {
            return await _context.DoctorSchedules.FirstOrDefaultAsync(x => x.DocId == doctorSchedule.DocId && x.WorkDate == doctorSchedule.WorkDate && x.SlotId == doctorSchedule.SlotId);
        }

        public async Task<DoctorSchedule> RegisterDoctorSchedule(DoctorSchedule doctorSchedule)
        {
            var doctorModel = _context.DoctorSchedules.Add(doctorSchedule);
            await _context.SaveChangesAsync();
            return doctorModel.Entity;
        }
    }
}
