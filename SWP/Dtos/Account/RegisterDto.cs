using System.ComponentModel.DataAnnotations;

namespace SWP.Dtos.Account
{
    public class RegisterDto
    {
        [Required]

        public string AccName { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Mật khẩu cần từ 8 chữ số trở lên vui lòng thử lại.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MinLength(10, ErrorMessage = "Sai số điện thoại.")]
        public string? Phone { get; set; }

        public string? Mail { get; set; }
    }
}
