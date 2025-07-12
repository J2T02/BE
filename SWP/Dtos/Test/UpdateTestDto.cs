using System.ComponentModel.DataAnnotations;

namespace SWP.Dtos.Test
{
    public class UpdateTestDto
    {
        public string? ResultDate { get; set; }
        public string? Note { get; set; }           
        public string FilePath { get; set; }
        public int Status { get; set; }

        public int TestType { get; set; }
        public int TestQualityStatus { get; set; }
    }
}
