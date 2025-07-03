using SWP.Dtos.Account;
using SWP.Dtos.Doctor;
using SWP.Models;
using SWP.Dtos.DoctorSchedule;

namespace SWP.Mapper
{
    public static class DoctorMappers
    {
        public static DoctorDto ToDoctorDto(this Doctor doctor)
        {
            var status = doctor.StatusNavigation;
            var edu = doctor.Edu;
            var cer = doctor.Certificates.Where(x => x.DocId == doctor.DocId);
            return new DoctorDto
            {
                //Field
                DocId = doctor.DocId,
                Gender = doctor.Gender,
                Yob = doctor.Yob,
                Experience = doctor.Experience,
                Status = new StatusInfoDto
                {
                    StatusId = status.StatusId,
                    StatusName = status.StatusName
                },
                EduInfo = new EduInfoDto
                {
                    EduId = edu.EduId,
                    EduName = edu.EduName,
                },
                CertificateInfo = cer.Select(x => new CertificateDto
                {
                    CerId = x.CerId,
                    CerName = x.CerName,
                    FilePath = x.FilePath,
                }).ToList()
                ,
                CreateAt = doctor.Acc.CreateAt,
                Img = doctor.Acc.Img,
                AccountInfo = new AccountDetailResponeDto
                {
                    AccId = doctor.Acc.AccId,
                    FullName = doctor.Acc.FullName,
                    Mail = doctor.Acc.Mail,
                    Phone = doctor.Acc.Phone
                }
            };
        }
        public static Doctor ToDoctorFromCreateDTO(this CreateDocotorRequestDto doctorDto)
        {
            return new Doctor
            {
                Gender = doctorDto.Gender,
                Yob = doctorDto.Yob,
                Experience = doctorDto.Experience,
                EduId = doctorDto.Edu_Id,
                Status = doctorDto.Status
            };
        }
        public static DoctorScheduleDto ToDoctorScheduleDto(this DoctorSchedule doctorSchedule)
        {
            if (doctorSchedule == null)
                return null;

            var doctor = doctorSchedule.Doc;
            var account = doctor?.Acc;
            var slot = doctorSchedule.Slot;

            return new DoctorScheduleDto
            {
                DsId = doctorSchedule.DsId,
                WorkDate = doctorSchedule.WorkDate,
                Slot = slot != null ? new SlotScheduleDto
                {
                    SlotId = slot.SlotId,
                    SlotStart = (TimeOnly)slot.SlotStart,
                    SlotEnd = (TimeOnly)slot.SlotEnd
                }
                : null,

                IsAvailable = doctorSchedule.IsAvailable,
                MaxBooking = doctorSchedule.MaxBooking
            };
        }

        public static DoctorSchedule ToDoctorScheduleFromCreateDTO(this CreateDoctorScheduleDto doctorSchedule, int docId)
        {
            return new DoctorSchedule
            {
                DocId = docId,
                WorkDate = doctorSchedule.WorkDate,
                SlotId = doctorSchedule.SlotId,
                IsAvailable = true,
                MaxBooking = doctorSchedule?.MaxBooking,
            };
        }
    }
}
