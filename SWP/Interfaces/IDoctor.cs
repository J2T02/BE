using SWP.Dtos.Doctor;
using SWP.Models;

namespace SWP.Interfaces
{
    public interface IDoctor
    {
        Task<List<Doctor>> GetAllDoctorsAsync();
        Task<Doctor> GetDoctorByIdAsync(int id);
        Task<Doctor> CreateDoctorAsync(Doctor doctor);
        Task<Doctor> UpdateDoctorAsync(int id, UpdateDoctorRequestDto doctor);
        Task<Doctor> DeleteDoctorAsync(int id);
    }
}
