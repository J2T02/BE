using System.ComponentModel.DataAnnotations;

namespace SWP.Dtos.Account
{
    public class RegisterDto
    {
        [Required]

        public string AccName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        
        public string? Phone { get; set; }
        public string? Mail { get; set; }
    }
}
