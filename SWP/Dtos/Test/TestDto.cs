using SWP.Dtos.Customer;
using SWP.Dtos.StepDetail;
using SWP.Dtos.TreatmentPlan;
using SWP.Models;

namespace SWP.Dtos.Test
{
    public class TestDto
    {
        public int TestId { get; set; }

        public TreatmentPlanInStepDetailDto TreatmenPlanInfo { get; set; }

        public TestTypeInfo TestType { get; set; }

        public StepDetailInfoDto StepDetail { get; set; }

        public DateOnly? TestDate { get; set; }

        public DateOnly? ResultDate { get; set; }
        public TestQualityStatusDto TestQualityStatus { get; set; }

        public string? Note { get; set; }

        public TestStatusDto Status { get; set; }
    }
}
