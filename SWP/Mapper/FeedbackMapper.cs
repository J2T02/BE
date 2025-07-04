using SWP.Dtos.Feedback;
using SWP.Models;

namespace SWP.Mapper
{
    public class FeedbackMapper
    {
        public static FeedbackDto ToDto(Feedback feedback)
        {
            return new FeedbackDto
            {
                DoctorId = feedback.DocId ?? 0,
                TreatmentPlanId = feedback.TpId ?? 0,
                star = feedback.Star,
                CreateAt = feedback.CreateAt ?? DateOnly.MinValue,
                Content = feedback.Content ?? string.Empty,
                Cus = feedback.Tp.Cus != null ? CustomerMapper.ToCustomerDto(feedback.Tp.Cus) : null
            };
        }

        public static List<FeedbackDto> ToDtoList(IEnumerable<Feedback> feedbacks)
        {
            return feedbacks.Select(f => ToDto(f)).ToList();
        }
    }
}
