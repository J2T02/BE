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
        private readonly ITreatmentPlan _treatmentPlanRepo;
        public StepDetailController(IStepDetail stepDetailRepo, IDoctor doctorRepo, ITreatmentPlan treatmentPlan)
        {
            _stepDetailRepo = stepDetailRepo;
            _doctorRepo = doctorRepo;
            _treatmentPlanRepo = treatmentPlan;
        }

        [Authorize(Roles = "Doctor, Receptionist")]
        [HttpPost("CreateStepDetail")]
        public async Task<IActionResult> CreateStepDetail([FromBody] CreateStepDetailDto request)
        {
            var checkTreatmentPlan = await _treatmentPlanRepo.GetTreatmentPlanById(request.TpId);
            if(checkTreatmentPlan == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Phác đồ điều trị không tồn tại", $"TreatmentPlanId: {request.TpId}"));
            }
            if (checkTreatmentPlan.Status == 2)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Phác đồ điều trị đã hoàn thành không được chỉnh sửa", $"TreatmentPlanId: {request.TpId}"));
            }
           var checkDsId = await _doctorRepo.GetDoctorScheduleByIdAsync(request.DsId);
            if (checkDsId == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Lịch khám không hợp lệ", $"DsId: {request.DsId}"));
            }
            var stepDetailModel = request.ToStepDetailFromCreate();
     

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
        [Authorize(Roles = "Doctor, Customer, Receptionist")]
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
        [Authorize(Roles = "Doctor, Customer, Receptionist")]
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
        [Authorize(Roles = "Doctor, Receptionist")]
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
        [Authorize(Roles = "Doctor, Receptionist")]
        [HttpPut("UpdateStepDetailStatus{id}")]
        public async Task<IActionResult> UpdateStepDetailStatus([FromRoute] int id, UpdateStatusDto request)
        {

            var checkStatusId = await _stepDetailRepo.GetStepDetailStatus(request.StatusId);
            if (checkStatusId == null)
            {
                return BadRequest(BaseRespone<StepDetailDto>.ErrorResponse("Trạng thái được chọn không hợp lệ", null, HttpStatusCode.BadRequest));
            }

            var result = await _stepDetailRepo.UpdateStepDetailStatus(id, request);
            if (result == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Cập nhật thất bại", "", HttpStatusCode.BadRequest));
            }
            var stepDetailModelDto = result.ToStepDetailDto();
            var response = BaseRespone<StepDetailDto>.SuccessResponse(stepDetailModelDto, "Cập nhật thành công", HttpStatusCode.OK);

            return Ok(response);
        }
    }
}
