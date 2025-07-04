using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP.Data;
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

        public FeedbackController(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
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

    }
}
