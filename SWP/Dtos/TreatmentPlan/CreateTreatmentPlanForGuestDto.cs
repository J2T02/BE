using System.ComponentModel.DataAnnotations;

namespace SWP.Dtos.TreatmentPlan
{
    public class CreateTreatmentPlanForGuestDto
    {
        [Required]  
        public string HusName { get; set; }
        public DateOnly HusYob { get; set; }
        public string WifeName { get; set; }
        public DateOnly WifeYob { get; set; }

        [Required]
        [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Sai số điện thoại")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Sai email.")]
        public string Mail { get; set; }

        public int DocId { get; set; }
        public int SerId { get; set; }
    }
}
