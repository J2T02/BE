using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Dtos.Services;
using SWP.Models;

namespace SWP.Dtos.TreatmentPlan
{
    public class TreatmentPlanDto
    {
        public int TpId { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public ServiceDto ServiceInfo { get; set; }

        public CustomerInTreatmentPlanDto CusInfo { get; set; }

        public DoctorAccountDto DoctorInfo { get; set; }

        public TreatmentPlanStatusDto Status { get; set; }

    }
}
