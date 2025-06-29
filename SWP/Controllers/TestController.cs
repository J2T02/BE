using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP.Data;
using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Dtos.StepDetail;
using SWP.Dtos.Test;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
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

        public TestController(ITest testRepo, ICustomerRepository customerRepo, IStepDetail stepDetailRepo, HIEM_MUONContext context)
        {
            _testRepo = testRepo;
            _customerRepo = customerRepo;
            _stepDetailRepo = stepDetailRepo;
            _context = context;
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
                return NotFound(BaseRespone<Test>.ErrorResponse("Dữ liệu không hợp lệ.", null, HttpStatusCode.NotFound));
            }
            var checkStatus = await _context.TestStatuses.FindAsync(request.Status);
            if (checkStatus == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Trạng thái được chọn không hợp lệ", $"Status: {request.Status}", HttpStatusCode.BadRequest));
            }
            var checkTestType = await _context.TestTypes.FindAsync(request.TestTypeId);
            if (checkTestType == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Loại xét nghiệm được chọn không hợp lệ", $"TestType: {request.TestType}", HttpStatusCode.BadRequest));
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


    }


}

