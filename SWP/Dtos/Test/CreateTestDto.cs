using System.ComponentModel.DataAnnotations;

namespace SWP.Dtos.Test
{
    public class CreateTestDto
    {
        [Required]
        public int CusId { get; set; }
        [Required]
        public int TestTypeId { get; set; }
        [Required]
        public int SdId { get; set; }
        [Required]
        //public string Result { get; set; } update sau khi test xong
        //TestDate phải lấy từ ngày hiện tại
        public string? Note { get; set; }
        [Required]
        public string FilePath { get; set; }
    }
}
