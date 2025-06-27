using SWP.Dtos.Customer;
using SWP.Dtos.StepDetail;
using SWP.Models;

namespace SWP.Dtos.Test
{
    public class HusTestDto
    {
        public int TestId { get; set; }

        public HusbandInfoDto CustomerInfo { get; set; }

        public TestTypeInfo TestType { get; set; }

        public StepDetailInfoDto StepDetail { get; set; }

        public DateOnly? TestDate { get; set; }

        public string Result { get; set; }

        public string Note { get; set; }

        public TestStatus Status { get; set; }
    }
}
