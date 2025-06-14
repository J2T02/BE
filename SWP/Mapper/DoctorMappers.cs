using SWP.Dtos.Doctor;
using SWP.Models;

namespace SWP.Mapper
{
    public static class DoctorMappers
    {
        public static DoctorDto ToDoctorDto(this Doctor doctor)
        {
            return new DoctorDto
            {
                DocId = doctor.DocId,
                AccId = doctor.AccId,
                DocName = doctor.DocName,
                Gender = doctor.Gender,
                Yob = doctor.Yob,
                Mail = doctor.Mail,
                Phone = doctor.Phone,
                Experience = doctor.Experience,
                Certification = doctor.Certification,
            };
        }
        public static Doctor ToDoctorFromCreateDTO(this CreateDocotorRequestDto doctorDto)
        {
            return new Doctor
            {
                DocName = doctorDto.DocName,
                Gender = doctorDto.Gender,
                Yob = doctorDto.Yob,
                Mail = doctorDto.Mail,
                Phone = doctorDto.Phone,
                Experience = doctorDto.Experience,
                Certification = doctorDto.Certification,
            };
        }
    }
}
