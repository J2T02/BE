using System.ComponentModel.DataAnnotations;

namespace SWP.Dtos.Customer
{
    public class UpdateCustomerRequestDto
    {
        public string HusName { get; set; }

        public string WifeName { get; set; }

        [Required]
        public DateOnly HusYob { get; set; }

        [Required]
        public DateOnly WifeYob { get; set; }
    }
}
