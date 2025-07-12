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
                Status = 1,
                DsId = request.DsId,
                DrugName = request.DrugName,
                Dosage = request.Dosage,
                DocId = request.DocId,
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
            var docSchedule = stepDetail.Ds;
            var cusAccount = customer?.Acc;
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
                        WifeYob = customer.WifeYob,
                        AccInfo = new AccountDetailResponeDto
                        {
                            AccId = cusAccount?.AccId ?? 0,
                            FullName = cusAccount?.FullName ?? "N/A",
                            Mail = cusAccount?.Mail ?? "N/A",
                            Phone = cusAccount?.Phone ?? "N/A"
                        }
                    },
                    ServiceInfo = service == null ? null : new ServiceInfoDto
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
                DocSchedule = new DocScheduleDto
                {
                    SlotId = docSchedule.SlotId,
                    WorkDate = docSchedule.WorkDate,
                },
            
                DrugName = stepDetail.DrugName,
                Dosage = stepDetail.Dosage
            };
        }

    }
}
