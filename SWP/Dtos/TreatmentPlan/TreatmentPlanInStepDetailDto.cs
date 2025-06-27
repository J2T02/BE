using SWP.Dtos.Customer;
using SWP.Dtos.Services;

namespace SWP.Dtos.TreatmentPlan
{
    public class TreatmentPlanInStepDetailDto
    {
        public int TpId { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public ServiceDto ServiceInfo { get; set; }

        public CustomerInfoDto CusInfo { get; set; }
        public TreatmentPlanStatusDto Status { get; set; }
    }
}
