using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP.Data;
using SWP.Dtos.Customer;
using SWP.Dtos.Doctor;
using SWP.Dtos.TreatmentPlan;
using SWP.Dtos.TreatmentStep;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
using System.Net;
using System.Security.Claims;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreamentPlanController : ControllerBase
    {
        private readonly HIEM_MUONContext _context;
        private readonly ITreatmentPlan _treatmentPlanRepo;
        private readonly IDoctor _doctorRepo;
        private readonly IServices _servicesRepo;
        private readonly ICustomerRepository _customerRepo;

        public TreamentPlanController(HIEM_MUONContext context, ITreatmentPlan treatmentPlanRepo, IDoctor doctorRepo, IServices services, ICustomerRepository customerRepo)
        {
            _context = context;
            _treatmentPlanRepo = treatmentPlanRepo;
            _doctorRepo = doctorRepo;
            _servicesRepo = services;
            _customerRepo = customerRepo;
        }

        [Authorize(Roles = "Doctor, Receptionist")]
        [HttpGet("GetAllTreatmentPlans")]
        public async Task<IActionResult> GetAllTreatmentPlan()
        {
            var treatmentPlans = await _treatmentPlanRepo.GetAllTreatmentPlans();

            if (treatmentPlans == null || !treatmentPlans.Any())
            {
                var error = BaseRespone<List<TreatmentPlanDto>>.ErrorResponse("Không tìm thấy phác đồ điều trị nào.", HttpStatusCode.NotFound);
                return NotFound(error);
            }

            var listDto = treatmentPlans.Select(x => x.ToTreatmentPlanDto()).ToList();
            var response = BaseRespone<List<TreatmentPlanDto>>.SuccessResponse(listDto, "Lấy danh sách thành công");
            return Ok(response);
        }
        [Authorize(Roles = "Doctor, Receptionist")]
        [HttpPost("CreateTreatmentPlan")]

        public async Task<IActionResult> CreateTreatmentPlan([FromBody] CreateTreatmentPlanDto dataRequest)
        {
            var checkService = await _servicesRepo.GetServiceById(dataRequest.SerId);
            if (checkService == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Dịch vụ được chọn không hợp lệ", $"Service Id: {dataRequest.SerId}", HttpStatusCode.BadRequest));
            }
            var checkCustomer = await _context.Customers.FirstOrDefaultAsync(x => x.CusId == dataRequest.CusId);
            if (checkCustomer == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Khách hàng được chọn không hợp lệ", $"Customer Id: {dataRequest.CusId}", HttpStatusCode.BadRequest));
            }
            var existingPlans = await _treatmentPlanRepo.GetTreatmentPlanByCustomerId(dataRequest.CusId);
            bool isCurrentlyInTreatment = existingPlans.Any(p => p.Status == 1);

            if (isCurrentlyInTreatment)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse(
                    "Khách hàng đang trong quá trình điều trị",
                    $"Customer Id: {dataRequest.CusId}",
                    HttpStatusCode.BadRequest
                ));
            }

            var doctorModel = await _doctorRepo.GetDoctorByIdAsync(dataRequest.DocId);
            if (doctorModel == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Không tìm thấy bác sĩ", $"Account Id: {dataRequest.DocId}", HttpStatusCode.BadRequest));
            }
            var treatmentPlanModel = dataRequest.ToTreatmentPlanFromCreate();
            treatmentPlanModel.Status = 1;
            treatmentPlanModel.StartDate = DateOnly.FromDateTime(DateTime.Now);
            treatmentPlanModel.Result = "Đang tiến hành";

            var result = await _treatmentPlanRepo.CreateTreatmentPlan(treatmentPlanModel);
            if (result == null) {
                return BadRequest(BaseRespone<TreatmentPlanDto>.ErrorResponse("Không thể tạo phác đồ điều trị", treatmentPlanModel.ToTreatmentPlanDto(), HttpStatusCode.BadRequest));
            }
            var responseDto = BaseRespone<TreatmentPlanDto>.SuccessResponse(result.ToTreatmentPlanDto(), "Tạo phác đồ điều trị thành công", HttpStatusCode.OK);
            return CreatedAtAction(nameof(GetTreatmentPlanById), new { id = result.TpId }, responseDto);
        }
        [Authorize(Roles = "Doctor, Customer, Receptionist")]
        [HttpGet("GetTreatmentPlanById/{id}")]
        public async Task<IActionResult> GetTreatmentPlanById([FromRoute] int id)
        {
            var treatmentPlanModel = await _treatmentPlanRepo.GetTreatmentPlanById(id);
            if (treatmentPlanModel == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Không tìm thấy phác đồ điều trị", $"TreatmentPlanId: {id}", HttpStatusCode.BadRequest));
            }
            var treatmentPlanDto = treatmentPlanModel.ToTreatmentPlanDto();
            return Ok(BaseRespone<TreatmentPlanDto>.SuccessResponse(treatmentPlanDto, "Lấy thông tin phác đồ điều trị thành công", HttpStatusCode.OK));
        }
        [Authorize(Roles = "Doctor, Customer, Receptionist")]
        [HttpGet("GetTreatmentPlanByCustomerId/{cusId}")]
        public async Task<IActionResult> GetTreatmentPlanByCusId([FromRoute] int cusId)
        {
            var checkCustomer = await _customerRepo.GetCustomerByCusIdAsync(cusId);
            if (checkCustomer == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Không tìm thấy khách hàng", $"Customer Id: {cusId}", HttpStatusCode.BadRequest));
            }
            var treatmentPlanModel = await _treatmentPlanRepo.GetTreatmentPlanByCustomerId(cusId);
            if (treatmentPlanModel == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Không tìm thấy phác đồ điều trị qua khách hàng", $"Customer Id: {cusId}", HttpStatusCode.BadRequest));
            }
            var treatmentPlanDto = treatmentPlanModel.Select(x => x.ToTreatmentPlanDto()).ToList();
            return Ok(BaseRespone<List<TreatmentPlanDto>>.SuccessResponse(treatmentPlanDto, "Lấy thông tin phác đồ điều trị thành công", HttpStatusCode.OK));
        }
        [Authorize(Roles = "Doctor, Customer, Receptionist")]
        [HttpGet("GetTreatmentPlanByDoctorId/{docId}")]
        public async Task<IActionResult> GetTreatmentPlanByDocId([FromRoute] int docId)
        {
            
            var treatmentPlanModel = await _treatmentPlanRepo.GetTreatmentPlanByDoctorId(docId);
            if (treatmentPlanModel == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Không tìm thấy phác đồ điều trị qua bác sĩ", $"Doctor Id: {docId}", HttpStatusCode.BadRequest));
            }
            var treatmentPlanDto = treatmentPlanModel.Select(x => x.ToTreatmentPlanDto()).ToList();
            return Ok(BaseRespone<List<TreatmentPlanDto>>.SuccessResponse(treatmentPlanDto, "Lấy thông tin phác đồ điều trị thành công", HttpStatusCode.OK));
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost("CreateTreatmentStep")]
        public async Task<IActionResult> CreateTreatmentStep([FromBody] CreateTreatmentStepDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

                var firstError = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage ?? "Dữ liệu không hợp lệ";
                var error = BaseRespone<DoctorDto>.ErrorResponse(firstError, HttpStatusCode.BadRequest);
                return BadRequest(error);
            }
            var treatmentStepModel = request.ToTreatmentStepFromCreate();
            var result = await _treatmentPlanRepo.CreateTreatmentStep(treatmentStepModel);
            if(result == null)
            {
                return BadRequest(BaseRespone<TreatmentStep>.ErrorResponse("Tạo bước điều trị thất bại", result, HttpStatusCode.BadRequest));
            }
            var treatmentStepDto = result.ToTreatmentStepDto();
            if (treatmentStepDto == null)
            {
                return BadRequest(BaseRespone<TreatmentStep>.ErrorResponse("Không tìm thấy thông tin bước điều trị", treatmentStepDto, HttpStatusCode.BadRequest));
            }
            var response = BaseRespone<TreatmentStepDto>.SuccessResponse(treatmentStepDto,"Lấy thông tin bước điều trị thành công",HttpStatusCode.OK);
            return CreatedAtAction(nameof(GetTreatmentStepById), new { id = result.TsId }, treatmentStepDto);
        }
        [Authorize(Roles = "Doctor,Customer")]
        [HttpGet("GetTreatmentStepById/{id}")]
        public async Task<IActionResult> GetTreatmentStepById([FromRoute] int id)
        {
            var treatmentStepModel = await _treatmentPlanRepo.GetTreatmentStepById(id);
            if(treatmentStepModel == null)
            {
                return BadRequest(BaseRespone<TreatmentStep>.ErrorResponse("Không tìm thấy thông tin bước điều trị", treatmentStepModel, HttpStatusCode.BadRequest));
            }
            var treatmentStepDto = treatmentStepModel.ToTreatmentStepDto();
            if (treatmentStepDto == null)
            {
                return BadRequest(BaseRespone<TreatmentStep>.ErrorResponse("Không tìm thấy thông tin bước điều trị", treatmentStepDto, HttpStatusCode.BadRequest));
            }
            var response = BaseRespone<TreatmentStepDto>.SuccessResponse(treatmentStepDto, "Lấy thông tin bước điều trị thành công", HttpStatusCode.OK);
            return Ok(response);
        }
        [Authorize(Roles = "Doctor, Receptionist")]
        [HttpPut("UpdateTreatmentPlan/{id}")]
        public async Task<IActionResult> UpdateTreatmentPlan([FromRoute] int id, UpdateTreatmentPlanDto request)
        {
            var checkServiceExist = await _servicesRepo.GetServiceById(request.SerId);
            if (checkServiceExist == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Dịch vụ được chọn không hợp lệ", $"Service Id: {request.SerId}", HttpStatusCode.BadRequest));
            }
            var checkTreatmentPlanStatus = await _treatmentPlanRepo.GetTreatmentPlanStatus(request.Status);
            if (checkTreatmentPlanStatus == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Trạng thái được chọn không hợp lệ", $"Status Id: {request.Status}", HttpStatusCode.BadRequest));
            }
            var result = await _treatmentPlanRepo.UpdateTreatmentPlan(id, request);
            if(result == null)
            {
                return BadRequest(BaseRespone<TreatmentPlanDto>.ErrorResponse("Cập nhật thất bại", null, HttpStatusCode.BadRequest));
            }
            var response = BaseRespone<TreatmentPlanDto>.SuccessResponse(result.ToTreatmentPlanDto(), "Cập nhật thành công");
            return Ok(response);
        }
    }
}
