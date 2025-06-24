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
                AccId = (int)(doctor.Acc.AccId),
                FullName = doctor.Acc.FullName,
                Mail = doctor.Acc.Mail,
                Phone = doctor.Acc.Phone,
                Cer_Id = (int)doctor.CerId,
                DocName = doctor.DocName,
                Gender = doctor.Gender,
                Yob = doctor.Yob,
                Experience = doctor.Experience,
                DoctorSchedule = doctor.DoctorSchedules.Where(x => x.WorkDate >= DateOnly.FromDateTime(DateTime.Today)).ToList()
            };
        }
        public static Doctor ToDoctorFromCreateDTO(this CreateDocotorRequestDto doctorDto)
        {
            return new Doctor
            {
                DocName = doctorDto.DocName,
                Gender = doctorDto.Gender,
                Yob = doctorDto.Yob,
                Experience = doctorDto.Experience,
                CerId = doctorDto.Cer_Id
            };
        }
        public static DoctorScheduleDto ToDoctorScheduleDto(this DoctorSchedule doctorSchedule)
        {
            return new DoctorScheduleDto
            {
                DsId = doctorSchedule.DsId,
                DocId = doctorSchedule.DocId,
                //DocName = doctorSchedule.Doc?.DocName,
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
