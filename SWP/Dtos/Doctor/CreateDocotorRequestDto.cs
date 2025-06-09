using System.ComponentModel.DataAnnotations;

namespace SWP.Dtos.Doctor
{
    public class CreateDocotorRequestDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Doctor name cannot exceed 50 characters.")]
        public string DocName { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]

        public string Gender { get; set; }
        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        [FutureDate(ErrorMessage = "Date must be in the future.")]
        public DateOnly? Yob { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Mail cannot exceed 50 characters.")]
        public string Mail { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits.")]
        [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Invalid Vietnamese phone number.")]
        public string Phone { get; set; }
        [Required]
        [Range(1,100, ErrorMessage ="Experience must be higher than 1")]
        public int? Experience { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Certification cannot exceed 50 characters.")]
        public string Certification { get; set; }
    }
}
