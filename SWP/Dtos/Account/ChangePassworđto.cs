using System.ComponentModel.DataAnnotations;

namespace SWP.Dtos.Account
{
    public class ChangePassworđto
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại không được để trống.")]
        [MinLength(8, ErrorMessage = "Mật khẩu hiện tại phải có ít nhất 8 ký tự.")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "Mật khẩu mới không được để trống.")]
        [MinLength(8, ErrorMessage = "Mật khẩu mới phải có ít nhất 8 ký tự.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
            ErrorMessage = "Mật khẩu mới phải có ít nhất 1 chữ và 1 số.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống.")]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không khớp với mật khẩu mới.")]
        public string ConfirmPassword { get; set; }
    }
}
