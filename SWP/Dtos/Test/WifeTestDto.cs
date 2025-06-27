using SWP.Dtos.Customer;
using SWP.Dtos.StepDetail;
using SWP.Models;

namespace SWP.Dtos.Test
{
    public class WifeTestDto
    {
        public int TestId { get; set; }

        public WifeInfoDto CustomerInfo { get; set; }

        public TestTypeInfo TestType { get; set; }

        public StepDetailInfoDto StepDetail { get; set; }

        public DateOnly? TestDate { get; set; }

        public string Result { get; set; }

        public string Note { get; set; }

        public TestStatus Status { get; set; }
    }
}
