using SWP.Dtos.Account;
using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Dtos.Services;
using SWP.Dtos.TreatmentPlan;
using SWP.Dtos.TreatmentStep;
using SWP.Models;

namespace SWP.Mapper
{
    public static class TreatmentPlanMapper
    {
        public static TreatmentPlan ToTreatmentPlanFromCreate(this CreateTreatmentPlanDto dto)
        {
            return new TreatmentPlan
            {
                EndDate = dto.EndDate,
                SerId = dto.SerId,
                CusId = dto.CusId,
            };
        }
        public static TreatmentPlanDto ToTreatmentPlanDto(this TreatmentPlan treatmentPlan)
        {
            var doctor = treatmentPlan.Doc;
            var account = doctor?.Acc;
            var cusInfo = treatmentPlan.Cus;
            var status = treatmentPlan.StatusNavigation;
            return new TreatmentPlanDto
            {
                TpId = treatmentPlan.TpId,
                StartDate = treatmentPlan.StartDate,
                EndDate = treatmentPlan.EndDate,
                ServiceInfo = new ServiceDto
                {
                    SerId = (int)treatmentPlan.SerId,
                    SerName = treatmentPlan.Ser?.SerName ?? "N/A",
                },
                CusInfo = new CustomerInTreatmentPlanDto
                {
                    HusName = cusInfo.HusName ?? "N/A",
                    HusYob = cusInfo.HusYob ?? null,
                    WifeName = cusInfo.WifeName ?? "N/A",
                    WifeYob = cusInfo.WifeYob ?? null,
                },
                DoctorInfo = new DoctorAccountDto
                {
                    DocId = (int)treatmentPlan.DocId,
                    AccountInfo = new AccountDetailResponeDto
                    {
                        FullName = account?.FullName ?? "N/A",
                        Mail = account?.Mail ??"N/A",
                        Phone = account?.Phone ?? "N/A"
                    }
                },
                Status = new TreatmentPlanStatusDto
                {
                    StatusId = status.StatusId,
                    StatusName = status.StatusName
                }
            };

        }
        public static TreatmentStep ToTreatmentStepFromCreate(this CreateTreatmentStepDto treatmentStep)
        {
            return new TreatmentStep
            {
                StepName = treatmentStep.StepName,
                Description = treatmentStep.Description,
                SerId = treatmentStep.SerId,
            };
        }
        public static TreatmentStepDto ToTreatmentStepDto(this TreatmentStep treatmentStep)
        {
            return new TreatmentStepDto
            {
                TsId = treatmentStep.TsId,
                StepName = treatmentStep.StepName,
                Description = treatmentStep.Description,
                ServiceInfo = new ServiceDto
                {
                    SerId = (int)treatmentStep.SerId,
                    SerName = treatmentStep.Ser.SerName
                }
            };
        }
    }
}
