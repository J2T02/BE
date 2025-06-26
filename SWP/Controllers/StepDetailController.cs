using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP.Data;
using SWP.Dtos.StepDetail;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
using SWP.Repository;
using System.Net;
using System.Security.Claims;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StepDetailController : ControllerBase
    {
        private readonly IStepDetail _stepDetailRepo;
        private readonly IDoctor _doctorRepo;

        public StepDetailController(IStepDetail stepDetailRepo, IDoctor doctorRepo)
        {
            _stepDetailRepo = stepDetailRepo;
            _doctorRepo = doctorRepo;
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("CreateStepDetail")]
        public async Task<IActionResult> CreateStepDetail([FromBody] CreateStepDetailDto request)
        {
            var stepDetailModel = request.ToStepDetailFromCreate();
            var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (accountIdClaim == null)
            {
                return BadRequest(BaseRespone<TreatmentPlan>.ErrorResponse("Không tìm thấy thông tin tài khoản", null, HttpStatusCode.BadRequest));
            }

            int accountId = int.Parse(accountIdClaim);
            var doctorModel = await _doctorRepo.GetDoctorByAccountId(accountId);
            stepDetailModel.DocId = doctorModel.DocId;
            var result = await _stepDetailRepo.CreateStepDetail(stepDetailModel);
            if (result == null)
            {
                return BadRequest(BaseRespone<StepDetail>.ErrorResponse("Tạo bước điều trị thất bại", result, HttpStatusCode.BadRequest));
            }
            var StepDetailDto = result.ToStepDetailDto();
            if (StepDetailDto == null)
            {
                return BadRequest(BaseRespone<StepDetailDto>.ErrorResponse("Không tìm thấy thông tin chi tiết bước điều trị", StepDetailDto, HttpStatusCode.BadRequest));
            }
            var responseDto = BaseRespone<StepDetailDto>.SuccessResponse(StepDetailDto, "Tạo chi tiết bước điều trị thành công", HttpStatusCode.OK);

            return CreatedAtAction(nameof(GetStepDetailById), new { id = result.SdId }, responseDto);
        }
        [Authorize(Roles = "Doctor, Customer")]
        [HttpGet("GetStepDetailById/{id}")]
        public async Task<IActionResult> GetStepDetailById([FromRoute] int id)
        {
            var result = await _stepDetailRepo.GetStepDetailById(id);
            if (result == null)
            {
                return BadRequest(BaseRespone<StepDetail>.ErrorResponse("Không tìm thấy thông tin chi tiết bước điều trị", result, HttpStatusCode.BadRequest));
            }
            var stepDetail = result.ToStepDetailDto();
            if (stepDetail == null)
            {
                return BadRequest(BaseRespone<StepDetailDto>.ErrorResponse("Không tìm thấy thông tin chi tiết bước điều trị", stepDetail, HttpStatusCode.BadRequest));
            }
            var responseDto = BaseRespone<StepDetailDto>.SuccessResponse(stepDetail, "Lấy thông tin chi tiết bước điều trị thành công", HttpStatusCode.OK);
            return Ok(responseDto);
        }
        [Authorize(Roles = "Doctor, Customer")]
        [HttpGet("GetAllStepDetailByTreatmentPlanId/{treatmentPlanId}")]
        public async Task<IActionResult> GetAllStepDetailByTreatmentPlanId([FromRoute] int treatmentPlanId)
        {
            var resultList = await _stepDetailRepo.GetAllStepDetailByTreatmentPlanId(treatmentPlanId);
            if (resultList == null)
            {
                return BadRequest(BaseRespone<List<StepDetail>>.ErrorResponse("Không tìm thấy danh sách thông tin chi tiết bước điều trị trong phác đồ", resultList, HttpStatusCode.BadRequest));
            }
            var resultListDto = resultList.Select(x => x.ToStepDetailDto()).ToList();
            var response = BaseRespone<List<StepDetailDto>>.SuccessResponse(resultListDto, "Lấy danh sách thông tin chi tiết bước điều trị trong phác đồ thành công", HttpStatusCode.OK);
            return Ok(response);
        }
        [Authorize(Roles = "Doctor")]
        [HttpPut("UpdateStepDetail/{id}")]
        public async Task<IActionResult> UpdateStepDetail([FromRoute] int id, UpdateStepDetailDto request)
        {
            var checkDoctorExist = await _doctorRepo.GetDoctorByIdAsync(request.DocId);
            if (checkDoctorExist == null)
            {
                return BadRequest(BaseRespone<StepDetailDto>.ErrorResponse("Bác sĩ được chọn không hợp lệ", null, HttpStatusCode.BadRequest));
            }
            var checkStatusId = await _stepDetailRepo.GetStepDetailStatus(request.Status);
            if (checkStatusId == null)
            {
                return BadRequest(BaseRespone<StepDetailDto>.ErrorResponse("Trạng thái được chọn không hợp lệ", null, HttpStatusCode.BadRequest));
            }

            var result = await _stepDetailRepo.UpdateStepDetail(id, request);
            if (result == null)
            {
                return BadRequest(BaseRespone<StepDetailDto>.ErrorResponse("Cập nhật thất bại", result, HttpStatusCode.BadRequest));
            }
            var stepDetailModelDto = result.ToStepDetailDto();
            var response = BaseRespone<StepDetailDto>.SuccessResponse(stepDetailModelDto, "Cập nhật thành công", HttpStatusCode.OK);
            
            return Ok(response);
        }
    }
}
