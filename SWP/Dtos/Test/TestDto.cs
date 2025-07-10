using SWP.Dtos.Customer;
using SWP.Dtos.StepDetail;
using SWP.Models;

namespace SWP.Dtos.Test
{
    public class TestDto
    {
        public int TestId { get; set; }

        public CustomerInfoDto CusInfo { get; set; }

        public TestTypeInfo TestType { get; set; }

        public StepDetailInfoDto StepDetail { get; set; }

        public DateOnly? TestDate { get; set; }

        public DateOnly? ResultDate { get; set; }

        public string Note { get; set; }

        public TestStatus Status { get; set; }
    }
}
