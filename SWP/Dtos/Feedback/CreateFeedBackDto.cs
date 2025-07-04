using System.ComponentModel.DataAnnotations;

namespace SWP.Dtos.Feedback
{
    public class CreateFeedBackDto
    {
        [Required]
        public int TpId { get; set; }
        [Required]
        public int DocId { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage ="Số sao phải lớn hơn bằng 1 và bé hơn bằng 5")]
        public int Star { get; set; }

        public string Content { get; set; }

    }
}
