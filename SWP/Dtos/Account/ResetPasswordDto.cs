using System.ComponentModel.DataAnnotations;

namespace SWP.Dtos.Account
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email hoặc số điện thoại không được để trống.")]
        public string EmailOrPhone { get; set; }

        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
            ErrorMessage = "Mật khẩu phải có ít nhất 1 chữ và 1 số.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Mã OTP không được để trống.")]
        public string Otp { get; set; }
    }
}
