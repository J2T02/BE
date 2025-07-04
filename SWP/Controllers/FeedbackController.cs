using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP.Data;
using SWP.Dtos.Doctor;
using SWP.Dtos.Feedback;
using SWP.Interfaces;
using SWP.Mapper;
using System.Net;

namespace SWP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly ITreatmentPlan _treatmentPlanRepo;
        private readonly IDoctor _doctorRepo;

        public FeedbackController(IFeedbackRepository feedbackRepository, ITreatmentPlan treatmentPlanRepo, IDoctor doctorRepo)
        {
            _feedbackRepository = feedbackRepository;
            _treatmentPlanRepo = treatmentPlanRepo;
            _doctorRepo = doctorRepo;
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<BaseRespone<List<FeedbackDto>>> GetFeedbacksByDoctorId(int doctorId)
        {
            try
            {
                var feedbacks = await _feedbackRepository.GetFeedbacksByDoctorIdAsync(doctorId);

                if (feedbacks == null || !feedbacks.Any())
                {
                    return new BaseRespone<List<FeedbackDto>>(
                        statusCode: HttpStatusCode.NotFound,
                        message: "Không tìm thấy Feedback cho Doctor này"
                    );
                }

                var feedbackDtos = FeedbackMapper.ToDtoList(feedbacks);

                return new BaseRespone<List<FeedbackDto>>(
                    data: feedbackDtos,
                    message: "Lấy dữ liệu thành công",
                    statusCode: HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                return new BaseRespone<List<FeedbackDto>>(
                    statusCode: HttpStatusCode.InternalServerError,
                    message: "Lỗi: " + ex.Message
                );
            }
        }

        [HttpGet("TreatmentPlan/{treatmentPlanId}")]
        public async Task<BaseRespone<List<FeedbackDto>>> GetFeedbacksByTreatmentPlanId(int treatmentPlanId)
        {
            try
            {
                var feedbacks = await _feedbackRepository.GetFeedbacksByTreatmentPlanIdAsync(treatmentPlanId);

                if (feedbacks == null || !feedbacks.Any())
                {
                    return new BaseRespone<List<FeedbackDto>>(
                        statusCode: HttpStatusCode.NotFound,
                        message: "Không tìm thấy feedback của TreatmentPlan này"
                    );
                }

                var feedbackDtos = FeedbackMapper.ToDtoList(feedbacks);

                return new BaseRespone<List<FeedbackDto>>(
                    data: feedbackDtos,
                    message: "Lấy dữ liệu thành công",
                    statusCode: HttpStatusCode.OK
                );
            }
            catch (Exception ex)
            {
                return new BaseRespone<List<FeedbackDto>>(
                    statusCode: HttpStatusCode.InternalServerError,
                    message: "Lỗi: " + ex.Message
                );
            }
        }
        [HttpGet("GetFeedbackById/{id}")]
        public async Task<IActionResult> GetFeedbackById([FromRoute] int id)
        {
            var result = await _feedbackRepository.GetFeedbackById(id);
            if (result == null)
            {
                return NotFound(BaseRespone<string>.ErrorResponse("Không tìm thấy ý kiến phản hồi", $"Feedback Id: {id}", HttpStatusCode.NotFound));
            }
            var feedbackDto = result.ToDto();
            return Ok(BaseRespone<FeedbackDto>.SuccessResponse(feedbackDto, "Lấy thông tin ý kiến phản hồi thành công"));
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> PostFeedBack([FromBody] CreateFeedBackDto request)
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
            var checkTreatmentPlan = await _treatmentPlanRepo.GetTreatmentPlanById(request.TpId);
            if (checkTreatmentPlan == null)
            {
                return NotFound(BaseRespone<string>.ErrorResponse("Không tìm thấy ý phác đồ điều trị", $"TreatmentPlan Id: {request.TpId}", HttpStatusCode.NotFound));
            }
            var checkDoctor = await _doctorRepo.GetDoctorByIdAsync(request.DocId);
            if (checkDoctor == null)
            {
                return NotFound(BaseRespone<string>.ErrorResponse("Không tìm thấy bác sĩ", $"Doctor Id: {request.DocId}", HttpStatusCode.NotFound));
            }
            var feedbackModel = request.ToFeedbackFromCreate();
            var result = await _feedbackRepository.PostFeedback(feedbackModel);
            if (result == null)
            {
                return BadRequest(BaseRespone<string>.ErrorResponse("Gửi ý kiến phản hồi thất bại", "", HttpStatusCode.BadRequest));
            }
            var response = BaseRespone<FeedbackDto>.SuccessResponse(result.ToDto(), "Gửi ý kiến phản hồi thành công");
            return CreatedAtAction(nameof(GetFeedbackById), new { id = result.FbId }, response);
        }

    }
}
