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
                AccId = (int)(doctor.Acc?.AccId),
                AccName = doctor.Acc?.AccName,
                DocName = doctor.DocName,
                Gender = doctor.Gender,
                Yob = doctor.Yob,
                Mail = doctor.Mail,
                Phone = doctor.Phone,
                Experience = doctor.Experience,
                Certification = doctor.Certification,
                DoctorSchedule = doctor.DoctorSchedules.Where(x => x.WorkDate >= DateOnly.FromDateTime(DateTime.Today)).ToList()
            };
        }
        public static Doctor ToDoctorFromCreateDTO(this CreateDocotorRequestDto doctorDto)
        {
            return new Doctor
            {
                DocName = doctorDto.DocName,
                Phone = doctorDto.Phone,
                Mail = doctorDto.Mail,
                Gender = doctorDto.Gender,
                Yob = doctorDto.Yob,
                Experience = doctorDto.Experience,
                Certification = doctorDto.Certification,
            };
        }
        public static DoctorScheduleDto ToDoctorScheduleDto(this DoctorSchedule doctorSchedule)
        {
            return new DoctorScheduleDto
            {
                DsId = doctorSchedule.DsId,
                DocId = doctorSchedule.DocId,
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
