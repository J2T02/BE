using SWP.Dtos.Feedback;
using SWP.Models;
using System;

namespace SWP.Mapper
{
    public static class FeedbackMapper
    {
        public static FeedbackDto ToDto(this Feedback feedback)
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
        public static Feedback ToFeedbackFromCreate(this CreateFeedBackDto request)
        {
            return new Feedback
            {
                TpId = request.TpId,
                DocId = request.DocId,
                Star = request.Star,
                CreateAt = DateOnly.FromDateTime(DateTime.Now),
                Content = request.Content
            };
        }
        public static List<FeedbackDto> ToDtoList(IEnumerable<Feedback> feedbacks)
        {
            return feedbacks.Select(f => ToDto(f)).ToList();
        }
    }
}
