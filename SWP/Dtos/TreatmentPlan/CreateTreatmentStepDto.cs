using System.ComponentModel.DataAnnotations;

namespace SWP.Dtos.TreatmentPlan
{
    public class CreateTreatmentStepDto
    {
        [Required]
        [MaxLength(100, ErrorMessage ="Tên không vượt quá 100 ký tự")]
        public string StepName { get; set; }

        public string Description { get; set; }
        [Required]
        [Range(1,2, ErrorMessage ="Dịch vụ không hỗ trợ")]
        public int? SerId { get; set; }

    }
}
