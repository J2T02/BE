using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP.Data;
using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
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

        public TestController(ITest testRepo, ICustomerRepository customerRepo, IStepDetail stepDetailRepo)
        {
            _testRepo = testRepo;
            _customerRepo = customerRepo;
            _stepDetailRepo = stepDetailRepo;
        }

        [Authorize(Roles = "Doctor, Customer")]
        [HttpGet("GetTestById")]
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
            
            var checkCus = await _customerRepo.GetCustomerByIdAsync(request.CusId);
            var checkStepDetail = await _stepDetailRepo.GetStepDetailById(request.SdId);
            if (checkCus == null || checkStepDetail == null)
            {
                return NotFound(BaseRespone<Test>.ErrorResponse("Dữ liệu không hợp lệ.", null, HttpStatusCode.NotFound));
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

    }


}

