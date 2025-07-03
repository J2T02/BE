using SWP.Dtos.Account;
using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Dtos.Services;
using SWP.Dtos.StepDetail;
using SWP.Dtos.TreatmentPlan;
using SWP.Dtos.TreatmentStep;
using SWP.Models;

namespace SWP.Mapper
{
    public static class StepDetailMapper
    {
        public static StepDetail ToStepDetailFromCreate(this CreateStepDetailDto request)
        {
            return new StepDetail
            {
                TpId = request.TpId,
                TsId = request.TsId,
                StepName = request.StepName,
                Note = request.Note,
                Status = request.Status,
                PlanDate = request.PlanDate,
                DoneDate = request.DoneDate,
                DrugName = request.DrugName,
                Dosage = request.Dosage
            };
        }
        public static StepDetailDto ToStepDetailDto(this StepDetail stepDetail)
        {
            var treatmentPlan = stepDetail.Tp;
            var customer = treatmentPlan?.Cus;
            var service = treatmentPlan?.Ser;
            var statusTreatmentPlan = treatmentPlan?.StatusNavigation;
            var treatmentStep = stepDetail.Ts;
            var doctor = stepDetail.Doc;
            var account = doctor?.Acc;
            var statusStepDetail = stepDetail.StatusNavigation;

            return new StepDetailDto
            {
                SdId = stepDetail.SdId,
                TreatmentPlanInfo = treatmentPlan == null ? null : new TreatmentPlanInStepDetailDto
                {
                    TpId = treatmentPlan.TpId,
                    StartDate = treatmentPlan.StartDate,
                    EndDate = treatmentPlan.EndDate,
                    CusInfo = customer == null ? null : new CustomerInfoDto
                    {
                        HusName = customer.HusName ?? "N/A",
                        HusYob = customer.HusYob,
                        WifeName = customer.WifeName ?? "N/A",
                        WifeYob = customer.WifeYob
                    },
                    ServiceInfo = service == null ? null : new ServiceDto
                    {
                        SerId = service.SerId,
                        SerName = service.SerName ?? "N/A"
                    },
                    Status = statusTreatmentPlan == null ? null : new TreatmentPlanStatusDto
                    {
                        StatusId = statusTreatmentPlan.StatusId,
                        StatusName = statusTreatmentPlan.StatusName
                    }
                },
                TreatmentStepInfo = treatmentStep == null ? null : new TreatmentStepInStepDetailDto
                {
                    TsId = treatmentStep.TsId,
                    StepName = treatmentStep.StepName
                },
                DoctorInfo = doctor == null ? null : new DoctorAccountDto
                {
                    DocId = doctor.DocId,
                    AccountInfo = account == null ? null : new AccountDetailResponeDto
                    {
                        AccId = account.AccId,
                        FullName = account.FullName ?? "N/A",
                        Mail = account.Mail ?? "N/A",
                        Phone = account.Phone ?? "N/A"
                    }
                },
                StepName = stepDetail.StepName,
                Note = stepDetail.Note,
                Status = statusStepDetail == null ? null : new StepDetailStatusDto
                {
                    StatusId = statusStepDetail.StatusId,
                    StatusName = statusStepDetail.StatusName
                },
                PlanDate = stepDetail.PlanDate,
                DoneDate = stepDetail.DoneDate,
                DrugName = stepDetail.DrugName,
                Dosage = stepDetail.Dosage
            };
        }

    }
}
