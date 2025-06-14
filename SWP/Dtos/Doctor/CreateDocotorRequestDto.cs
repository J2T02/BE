using System.ComponentModel.DataAnnotations;

namespace SWP.Dtos.Doctor
{
    public class CreateDocotorRequestDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Tên không được vượt quá 50 kí tự")]
        public string DocName { get; set; }

        [Required]
        [MaxLength(15, ErrorMessage = "Giới tính không được vượt quá 15 kí tự")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Ngày sinh là thông tin bắt buộc")]
        [DataType(DataType.Date, ErrorMessage = "Định dạng ngày tháng không hợp lệ.")]
        [PastDate(ErrorMessage = "Ngày sinh phải là ngày trong quá khứ")]
        [MinAge(18, ErrorMessage = "Bạn phải ít nhất 18 tuổi trở lên.")]
        public DateOnly? Yob { get; set; }

        [Required]
        [EmailAddress(ErrorMessage ="Định dạng Mail không hợp lệ")]
        public string Mail { get; set; }

        [Required(ErrorMessage = "Số điện thoại là thông tin bắt buộc.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại yêu cầu 10 số")]
        [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; }

        [Required]
        [Range(0,60, ErrorMessage ="Kinh nghiệm làm việc không vượt quá 60 năm")]
        public int? Experience { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Bằng cấp không vượt quá 50 kí tự.")]
        public string Certification { get; set; }
    }
    public class PastDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateOnly date)
            {
                return date <= DateOnly.FromDateTime(DateTime.Today);
            }
            return true;
        }
    }
    public class MinAgeAttribute : ValidationAttribute
    {
        private readonly int _minAge;

        public MinAgeAttribute(int minAge)
        {
            _minAge = minAge;
        }

        public override bool IsValid(object? value)
        {
            if (value is DateOnly birthDate)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var age = today.Year - birthDate.Year;

                
                if (today < birthDate.AddYears(age))
                {
                    age--;
                }

                return age >= _minAge;
            }

            return true; 
        }
    }
}
