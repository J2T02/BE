using System.ComponentModel.DataAnnotations;
using SWP.Dtos.Doctor;

namespace SWP.Dtos.Account
{
    public class RegisterDto
    {
        [EmailAddress(ErrorMessage = "Sai email.")]
        public string Mail { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Mật khẩu cần từ 8 ký tự trở lên.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
        ErrorMessage = "Mật khẩu phải chứa ít nhất 8 ký tự, bao gồm cả chữ và số.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;


        [Required]
        [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Sai số điện thoại")]
        public string Phone { get; set; } = string.Empty;

        

        
    

    
    }
}
