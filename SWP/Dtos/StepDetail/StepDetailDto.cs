using SWP.Dtos.Doctor;
using SWP.Dtos.TreatmentPlan;
using SWP.Dtos.TreatmentStep;
using SWP.Models;

namespace SWP.Dtos.StepDetail
{
    public class StepDetailDto
    {
        public int SdId { get; set; }

        public string StepName { get; set; }

        public string Note { get; set; }

        public StepDetailStatusDto Status { get; set; }

        public DocScheduleDto DocSchedule { get; set; }

        public string DrugName { get; set; }

        public string Dosage { get; set; }

        public TreatmentPlanInStepDetailDto TreatmentPlanInfo { get; set; }

        public TreatmentStepInStepDetailDto TreatmentStepInfo { get; set; }

        public DoctorAccountDto DoctorInfo { get; set; }
    }
}
