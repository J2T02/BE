using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using SWP.Data;
using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Dtos.StepDetail;
using SWP.Dtos.Test;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
using System.Globalization;
using System.Net;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TestController : ControllerBase
    {
        private readonly ITest _testRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IStepDetail _stepDetailRepo;
        private readonly HIEM_MUONContext _context;
        private readonly ITreatmentPlan _treatmentPlanRepo;

        public TestController(ITest testRepo, ICustomerRepository customerRepo, IStepDetail stepDetailRepo, HIEM_MUONContext context, ITreatmentPlan treatmentPlan)
        {
            _testRepo = testRepo;
            _customerRepo = customerRepo;
            _stepDetailRepo = stepDetailRepo;
            _context = context;
            _treatmentPlanRepo = treatmentPlan;
        }

        [Authorize(Roles = "Doctor, Customer")]
        [HttpGet("GetTestById/{id}")]
        public async Task<IActionResult> GetTestById([FromRoute] int id)
        {
            var result = await _testRepo.GetTestById(id);
            if (result == null)
            {
                return NotFound(BaseRespone<string>.ErrorResponse("Không tìm thấy thông tin xét nghiệm.", $"TestId: {id}", HttpStatusCode.NotFound));
            }

            if (result.TestType.TestName.Equals("wife")){
                var wifiTestDto = result.WifeTestDto();
                return Ok(BaseRespone<WifeTestDto>.SuccessResponse(wifiTestDto, "Lấy thông tin xét nghiệm thành công"));
            }
            else
            {
                var husTestDto = result.HusTestDto();
                return Ok(BaseRespone<HusTestDto>.SuccessResponse(husTestDto, "Lấy thông tin xét nghiệm thành công"));
            }

        }
        [Authorize(Roles = "Doctor")]
        [HttpPost("CreateTest")]
        public async Task<IActionResult> CreateTest([FromBody] CreateTestDto request)
        {
            
            var checkCus = await _customerRepo.GetCustomerByCusIdAsync(request.CusId);
            var checkStepDetail = await _stepDetailRepo.GetStepDetailById(request.SdId);
            if (checkCus == null || checkStepDetail == null)
            {
                return NotFound(BaseRespone<Test>.ErrorResponse("Dữ liệu khách hàng hoặc chi tiết bước điều trị không tồn tại.", null, HttpStatusCode.NotFound));
            }
            var checkTestType = await _context.TestTypes.FindAsync(request.TestTypeId);
            if (checkTestType == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Loại xét nghiệm được chọn không hợp lệ", $"TestType: {request.TestTypeId}", HttpStatusCode.BadRequest));
            }
            var testModel = request.ToTestFromCreate();
            testModel.TestDate = DateOnly.FromDateTime(DateTime.Now);
            var result = await _testRepo.CreateTest(testModel);
            if (result == null)
            {
                return NotFound(BaseRespone<Test>.ErrorResponse("Tạo xét nghiệm thất bại", testModel, HttpStatusCode.NotFound));
            }
            if (result.TestType.TestName.Equals("wife")){ 
                var wifiTestDto = result.WifeTestDto();
                var response = BaseRespone<WifeTestDto>.SuccessResponse(wifiTestDto, "Lấy thông tin xét nghiệm thành công");
                return CreatedAtAction(nameof(GetTestById), new { id = result.TestId }, response);
            }
            else
            {
                var husTestDto = result.HusTestDto();
                var response = BaseRespone<HusTestDto>.SuccessResponse(husTestDto, "Lấy thông tin xét nghiệm thành công");
                return CreatedAtAction(nameof(GetTestById), new { id = result.TestId }, response);
            }
        }
        [Authorize(Roles = "Doctor")]
        [HttpPut("UpdateTest/{id}")]
        public async Task<IActionResult> UpdateTest([FromRoute] int id,[FromBody] UpdateTestDto request)
        {
            bool isValid = DateOnly.TryParseExact(request.ResultDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly resultDate);

            if (!isValid)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Ngày không đúng định dạng yyyy-MM-dd", $"Date: {request.ResultDate}"));
            }
            var timeNow = DateOnly.FromDateTime(DateTime.Now);
            if (resultDate < timeNow)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Ngày kết quả không được là ngày trong quá khứ", $"ResultDate: {request.ResultDate}", HttpStatusCode.BadRequest));
            }
            var checkStatus = await _context.TestStatuses.FindAsync(request.Status);
            if (checkStatus == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Trạng thái được chọn không hợp lệ", $"Status: {request.Status}", HttpStatusCode.BadRequest));
            }
            var checkTestType = await _context.TestTypes.FindAsync(request.TestType);
            if (checkTestType == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Loại xét nghiệm được chọn không hợp lệ", $"TestType: {request.TestType}", HttpStatusCode.BadRequest));
            }

            var result = await _testRepo.UpdateTest(id, request);
            if (result == null)
            {
                return BadRequest(BaseRespone<TestDto>.ErrorResponse("Cập nhật thất bại", result.ToTestDto, HttpStatusCode.BadRequest));
            }
            var testModelDto = result.ToTestDto();
            var response = BaseRespone<TestDto>.SuccessResponse(testModelDto, "Cập nhật thành công", HttpStatusCode.OK);

            return Ok(response);
        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("GetTestByTreatmentPlanId/{treatmentPlanId}")]
        public async Task<IActionResult> GetTestByTreatmentPlanId([FromRoute] int treatmentPlanId)
        {
            var result = await _stepDetailRepo.GetAllStepDetailByTreatmentPlanId(treatmentPlanId);
            if (result == null || !result.Any())
            {
                return NotFound(BaseRespone<string>.ErrorResponse("Không tìm thấy thông tin phác đồ điều trị và các xét nghiệm.", $"TreatmentPlanId: {treatmentPlanId}", HttpStatusCode.NotFound));
            }
            var testList = new List<Test>();
            foreach (var stepDetail in result)
            {
                var tests = await _testRepo.GetTestByStepDetailId(stepDetail.SdId);
                if(tests != null && tests.Count != 0)
                {
                    foreach (var test in tests)
                    {
                        testList.Add(test);
                    }
                }
            }
            if(testList == null || !testList.Any())
            {
                return NotFound(BaseRespone<string>.ErrorResponse("Không tìm thấy thông tin xét nghiệm.", $"TreatmentPlanId: {treatmentPlanId}", HttpStatusCode.NotFound));
            }
            var testListDtos = testList.Select(x => x.ToTestDto()).ToList();
            return Ok(BaseRespone<List<TestDto>>.SuccessResponse(testListDtos, "Lấy thông tin xét nghiệm thành công", HttpStatusCode.OK));
        }
        [Authorize(Roles = "Doctor")]
        [HttpGet("GetTestByStepDetailId/{stepDetailId}")]
        public async Task<IActionResult> GetTestByStepDetailId([FromRoute] int stepDetailId)
        {
            var result = await _testRepo.GetTestByStepDetailId(stepDetailId);
            if (result == null || !result.Any())
            {
                return NotFound(BaseRespone<string>.ErrorResponse("Không tìm thấy thông tin chi tiết bước điều trị và các xét nghiệm.", $"StepDetailId: {stepDetailId}", HttpStatusCode.NotFound));
            }
            var testListDtos = result.Select(x => x.ToTestDto()).ToList();
            return Ok(BaseRespone<List<TestDto>>.SuccessResponse(testListDtos, "Lấy thông tin xét nghiệm thành công", HttpStatusCode.OK));
        }

    }


}

