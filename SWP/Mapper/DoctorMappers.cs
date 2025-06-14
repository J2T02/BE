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
        public static DoctorScheduleDto ToDoctorScheduleDto(this DoctorSchedule doctorSchedule)
        {
            return new DoctorScheduleDto
            {
                DocName = doctorSchedule.Doc?.DocName,
                WorkDate = doctorSchedule.WorkDate,
                SlotId = doctorSchedule.SlotId,
                IsAvailable = doctorSchedule.IsAvailable,
            };
        }
        public static DoctorSchedule ToDoctorScheduleFromCreateDTO(this CreateDoctorScheduleDto doctorSchedule)
        {
            return new DoctorSchedule
            {
                DocId = doctorSchedule.DocId,
                WorkDate = doctorSchedule?.WorkDate,
                SlotId = doctorSchedule.SlotId
            };
        }
    }
}
