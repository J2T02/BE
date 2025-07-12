using SWP.Dtos.Account;
using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Dtos.Services;
using SWP.Dtos.StepDetail;
using SWP.Dtos.Test;
using SWP.Dtos.TreatmentPlan;
using SWP.Models;

namespace SWP.Mapper
{
    public static class TestMapper
    {
        public static Test ToTestFromCreate(this CreateTestDto request)
        {

            return new Test
            {
                TpId = request.TpId,
                TestTypeId = request.TestTypeId,
                SdId = request.SdId,
                //TestDate = request.TestDate, //Lấy từ ngày hiện tại 
                TqsId = request.TqsId,
                //Result = request.Result,
                Note = request.Note,
                FilePath = request.FilePath,
                Status = 1
            };
        }

        public static TestDto ToTestDto(this Test request)
        {
            var testType = request.TestType;
            var stepDetail = request.Sd;
            var testStatus = request.StatusNavigation;
            var treatmentPlan = request.Tp;
            var customer = treatmentPlan?.Cus;
            var cusAccount = customer?.Acc;
            var service = treatmentPlan?.Ser;
            var tpStatus = treatmentPlan?.StatusNavigation;
            var doc = stepDetail?.Doc;
            var docAccount = doc?.Acc;
            var sdStatus = stepDetail?.StatusNavigation;
            var tqs = request.Tqs;

            return new TestDto
            {
                TestId = request.TestId,
                TreatmenPlanInfo = treatmentPlan == null ? null : new TreatmentPlanInStepDetailDto
                {
                    TpId = treatmentPlan.TpId,
                    ServiceInfo = service == null ? null : new ServiceInfoDto
                    {
                        SerId = service.SerId,
                        SerName = service.SerName ?? "N/A"
                    },
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
                    StartDate = treatmentPlan.StartDate,
                    EndDate = treatmentPlan.EndDate,
                    Status = tpStatus == null ? null : new TreatmentPlanStatusDto
                    {
                        StatusId = tpStatus.StatusId,
                        StatusName = tpStatus.StatusName
                    }
                },
                TestType = testType == null ? null : new TestTypeInfo
                {
                    Id = testType.TestTypeId,
                    Person = testType.Person,
                    TestName = testType.TestName,
                },
                StepDetail = stepDetail == null ? null : new StepDetailInfoDto
                {
                    SdId = stepDetail.SdId,
                    StepName = stepDetail.StepName,
                    Status = sdStatus == null ? null : new StepDetailStatusDto
                    {
                        StatusId = sdStatus.StatusId,
                        StatusName = sdStatus.StatusName
                    },
                    Note = stepDetail.Note,
                    DocInfo = doc == null ? null : new DoctorAccountDto
                    {
                        DocId = doc.DocId,
                        AccountInfo = docAccount == null ? null : new AccountDetailResponeDto
                        {
                            AccId = docAccount.AccId,
                            FullName = docAccount.FullName ?? "N/A",
                            Mail = docAccount.Mail ?? "N/A",
                            Phone = docAccount.Phone ?? "N/A"
                        }
                    }
                },
                TestDate = request.TestDate,
                ResultDate = request.ResultDay,
                Note = request.Note,
                Status = testStatus == null ? null : new TestStatusDto
                {
                    Id = testStatus.StatusId,
                    Name = testStatus.StatusName
                },
                TestQualityStatus = tqs == null ? null : new TestQualityStatusDto
                {
                    Id = tqs.TqsId,
                    Name = tqs.TqsName ?? "N/A",
                }
            };
        }

    }
}
