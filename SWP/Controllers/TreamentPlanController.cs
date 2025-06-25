using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP.Data;
using SWP.Dtos.Customer;
using SWP.Dtos.TreatmentPlan;
using SWP.Interfaces;
using SWP.Mapper;
using SWP.Models;
using System.Net;
using System.Security.Claims;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TreamentPlanController : Controller
    {
        private readonly HIEM_MUONContext _context;
        private readonly ITreatmentPlan _treatmentPlanRepo;
        private readonly IDoctor _doctorRepo;

        public TreamentPlanController(HIEM_MUONContext context, ITreatmentPlan treatmentPlanRepo, IDoctor doctorRepo)
        {
            _context = context;
            _treatmentPlanRepo = treatmentPlanRepo;
            _doctorRepo = doctorRepo;
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]

        public async Task<IActionResult> CreateTreatmentPlan([FromBody] CreateTreatmentPlanDto dataRequest)
        {
            var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (accountIdClaim == null)
            {
                return BadRequest(BaseRespone<TreatmentPlan>.ErrorResponse("Không tìm thấy thông tin tài khoản",null,HttpStatusCode.BadRequest));
            }

            int accountId = int.Parse(accountIdClaim);
            var doctorModel = await _doctorRepo.GetDoctorByAccountId(accountId);
            var treatmentPlanModel = dataRequest.ToTreatmentPlanFromCreate();
            treatmentPlanModel.DocId = doctorModel.DocId;
            treatmentPlanModel.Status = 1;

            var result = await _treatmentPlanRepo.CreateTreatmentPlan(treatmentPlanModel);
            if (result == null) {
                return BadRequest(BaseRespone<TreatmentPlanDto>.ErrorResponse("Không thể tạo phác đồ điều trị", treatmentPlanModel.ToTreatmentPlanDto(), HttpStatusCode.BadRequest));
            }
            var responseDto = BaseRespone<TreatmentPlanDto>.SuccessResponse(result.ToTreatmentPlanDto(), "Tạo phác đồ điều trị thành công", HttpStatusCode.OK);
            return CreatedAtAction(nameof(GetTreatmentPlanById), new { id = result.TpId }, responseDto);
        }
        [Authorize(Roles = "Doctor, Customer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTreatmentPlanById([FromRoute] int id)
        {
            var treatmentPlanModel = await _treatmentPlanRepo.GetTreatmentPlanById(id);
            if (treatmentPlanModel == null)
            {
                return BadRequest(BaseRespone<TreatmentPlan>.ErrorResponse("Không tìm thấy phác đồ điều trị", treatmentPlanModel, HttpStatusCode.BadRequest));
            }
            var treatmentPlanDto = treatmentPlanModel.ToTreatmentPlanDto();
            return Ok(BaseRespone<TreatmentPlanDto>.SuccessResponse(treatmentPlanDto, "Lấy thông tin phác đồ điều trị thành công", HttpStatusCode.OK));
        }
    }
}
