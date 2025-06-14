using System.ComponentModel.DataAnnotations;
using SWP.Models;

namespace SWP.Dtos.Doctor
{
    public class CreateDoctorScheduleDto
    {
        [Required]
        public int? DocId { get; set; }

        [Required(ErrorMessage ="Thông tin ngày làm việc là bắt buộc")]
        [DataType(DataType.Date, ErrorMessage = "Định dạng ngày tháng không hợp lệ.")]
        [CustomValidation(typeof(WorkDateValidator), nameof(WorkDateValidator.ValidateWorkDate))]
        public DateOnly? WorkDate { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "Vui lòng chọn một khung giờ hợp lệ.")]
        public int? SlotId { get; set; }
    }

    public static class WorkDateValidator
    {
        public static ValidationResult? ValidateWorkDate(DateOnly? workDate, ValidationContext context)
        {
            if (workDate == null)
            {
                return new ValidationResult("Ngày làm việc là bắt buộc.");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            if (workDate < today)
            {
                return new ValidationResult("Ngày làm việc không được ở quá khứ.");
            }

            return ValidationResult.Success;
        }
    }
    

    }
