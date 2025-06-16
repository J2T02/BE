using System.ComponentModel.DataAnnotations;
using SWP.Dtos.Doctor;

namespace SWP.Dtos.Account
{
    public class RegisterDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "Tên đăng nhập phải từ 5 kí tự trở lên.")]
        public string AccName { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Mật khẩu cần từ 8 ký tự trở lên.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
        ErrorMessage = "Mật khẩu phải chứa ít nhất 8 ký tự, bao gồm cả chữ và số.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Sai số điện thoại")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(50, ErrorMessage = "Tên không được vượt quá 50 kí tự")]
        public string HusName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Ngày sinh là thông tin bắt buộc")]
        [DataType(DataType.Date, ErrorMessage = "Định dạng ngày tháng không hợp lệ.")]
        [PastDate(ErrorMessage = "Ngày sinh phải là ngày trong quá khứ")]
        [MinAge(18, ErrorMessage = "Bạn phải ít nhất 18 tuổi trở lên.")]
        public DateOnly? HusYob { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Tên không được vượt quá 50 kí tự")]
        public string WifeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh là thông tin bắt buộc")]
        [DataType(DataType.Date, ErrorMessage = "Định dạng ngày tháng không hợp lệ.")]
        [PastDate(ErrorMessage = "Ngày sinh phải là ngày trong quá khứ")]
        [MinAge(18, ErrorMessage = "Bạn phải ít nhất 18 tuổi trở lên.")]
        public DateOnly WifeYob { get; set; }

        [EmailAddress(ErrorMessage = "Sai email.")]
        public string Mail { get; set; } = string.Empty;
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
