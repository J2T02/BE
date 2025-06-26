using SWP.Dtos.Services;

namespace SWP.Dtos.TreatmentStep
{
    public class TreatmentStepDto
    {
        public int TsId { get; set; }
        public string StepName { get; set; }
        public string Description { get; set; }
        public ServiceDto ServiceInfo { get; set; }
    }
}
