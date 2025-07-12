using SWP.Dtos.Customer;
using SWP.Dtos.Services;

namespace SWP.Dtos.TreatmentPlan
{
    public class TreatmentPlanInStepDetailDto
    {
        public int TpId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ServiceInfoDto ServiceInfo { get; set; }

        public CustomerInfoDto CusInfo { get; set; }
        public TreatmentPlanStatusDto Status { get; set; }
    }
}
