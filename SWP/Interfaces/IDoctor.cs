using SWP.Dtos.Doctor;
using SWP.Models;

namespace SWP.Interfaces
{
    public interface IDoctor
    {
        Task<List<Doctor>> GetAllDoctorsAsync();
        Task<Doctor> GetDoctorByIdAsync(int id);
        Task<Account> GetAccountByDoctor(Doctor doctor);
        Task<Doctor?> GetDoctorByAccountId(int accountId);
        Task<Doctor> CreateDoctorAsync(Doctor doctor);
        Task<Doctor> UpdateDoctorAsync(int id, UpdateDoctorRequestDto doctor);
        Task<string> DeleteDoctorAsync(int id);
        Task<DoctorSchedule> GetDoctorScheduleByIdAsync(int dsId);
        Task<DoctorSchedule> RegisterDoctorSchedule(DoctorSchedule doctorSchedule);
        Task<List<DoctorSchedule>> GetDoctorScheduleIsTrue(int doctorId);
        Task<List<DoctorSchedule>> GetAllDoctorSchedule(int doctorId);

        Task<SlotSchedule?> GetSlotById(int id);

        Task<List<DoctorSchedule>?> GetDoctorScheduleByDate(DateOnly request);

        Task<Doctor> GetDoctorByAccountIdAsync(int accountId);

        Task<DoctorStatus> GetDoctorStatusById(int id);
        Task<Doctor> UpdateDoctorStatus(int id, UpdateDoctorStatusDto request);
    }
}
