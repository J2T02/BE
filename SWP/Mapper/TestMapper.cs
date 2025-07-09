using SWP.Dtos.Customer;
using SWP.Dtos.StepDetail;
using SWP.Dtos.Test;
using SWP.Models;

namespace SWP.Mapper
{
    public static class TestMapper
    {
        public static Test ToTestFromCreate(this CreateTestDto request)
        {

            return new Test
            {
                CusId = request.CusId,
                TestTypeId = request.TestTypeId,
                SdId = request.SdId,
                //TestDate = request.TestDate, //Lấy từ ngày hiện tại 
                //Result = request.Result,
                Note = request.Note,
                FilePath = request.FilePath,
                Status = 1
            };
        }

        public static TestDto ToTestDto(this Test request)
        {
            var customer = request.Cus;
            var testType = request.TestType;
            var stepDetail = request.Sd;
            var testStatus = request.StatusNavigation;
            return new TestDto
            {
                TestId = request.TestId,
                CusInfo = new CustomerInfoDto
                {
                    HusName = customer.HusName,
                    HusYob = customer.HusYob,
                    WifeName = customer.WifeName,
                    WifeYob = customer.WifeYob,
                },
                TestType = new TestTypeInfo
                {
                    Id = testType.TestTypeId,
                    Person = testType.Person,
                    TestName = testType.TestName,
                },
                StepDetail = new StepDetailInfoDto
                {
                    SdId = stepDetail.SdId,
                    StepName = stepDetail.StepName,
                    Status = new StepDetailStatusDto
                    {
                        StatusId = stepDetail.StatusNavigation.StatusId,
                        StatusName = stepDetail.StatusNavigation.StatusName
                    },
                    Note = stepDetail.Note
                },
                TestDate = request.TestDate,
                ResultDate = request.ResultDay,
                Note = request.Note,
                Status = new TestStatus
                {
                    StatusId = testStatus.StatusId,
                    StatusName = testStatus.StatusName
                }
            };
        }
        public static HusTestDto HusTestDto(this Test request)
        {
            var customer = request.Cus;
            var testType = request.TestType;
            var stepDetail = request.Sd;
            var testStatus = request.StatusNavigation;
            return new HusTestDto
            {
                TestId = request.TestId,
                CustomerInfo = new HusbandInfoDto
                {
                    HusName = customer.HusName,
                    HusYob = customer.HusYob,
                },
                TestType = new TestTypeInfo
                {
                    Id = testType.TestTypeId,
                    Person = testType.Person,
                    TestName = testType.TestName,
                },
                StepDetail = new StepDetailInfoDto
                {
                    SdId = stepDetail.SdId,
                    StepName = stepDetail.StepName,
                    Status = new StepDetailStatusDto
                    {
                        StatusId = stepDetail.StatusNavigation.StatusId,
                        StatusName = stepDetail.StatusNavigation.StatusName
                    },
                    Note = stepDetail.Note
                },
                TestDate = request.TestDate,
                //Result = request.Result,
                Note = request.Note,
                Status = new TestStatus
                {
                    StatusId = testStatus.StatusId,
                    StatusName = testStatus.StatusName
                }
            };
        }
        public static WifeTestDto WifeTestDto(this Test request)
        {
            var customer = request.Cus;
            var testType = request.TestType;
            var stepDetail = request.Sd;
            var testStatus = request.StatusNavigation;
            return new WifeTestDto
            {
                TestId = request.TestId,
                CustomerInfo = new WifeInfoDto
                {
                    WifeName = customer.WifeName,
                    WifeYob = customer.WifeYob,
                },
                TestType = new TestTypeInfo
                {
                    Id = testType.TestTypeId,
                    Person = testType.Person,
                    TestName = testType.TestName,
                },
                StepDetail = new StepDetailInfoDto
                {
                    SdId = stepDetail.SdId,
                    StepName = stepDetail.StepName,
                    Status = new StepDetailStatusDto
                    {
                        StatusId = stepDetail.StatusNavigation.StatusId,
                        StatusName = stepDetail.StatusNavigation.StatusName
                    },
                    Note = stepDetail.Note
                },
                TestDate = request.TestDate,
                //Result = request.Result,
                Note = request.Note,
                Status = new TestStatus
                {
                    StatusId = testStatus.StatusId,
                    StatusName = testStatus.StatusName
                }
            };
        }
    }
}
