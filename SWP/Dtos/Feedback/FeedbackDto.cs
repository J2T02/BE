using SWP.Dtos.Customer;
using SWP.Models;
using System;

namespace SWP.Dtos.Feedback
{
    public class FeedbackDto
    {
        public int? star { get; set; }
        public DateOnly CreateAt { get; set; }
        public string? Content { get; set; }
        public int TreatmentPlanId { get; set; }
        public int DoctorId { get; set; }
        public virtual CustomerDto Cus { get; set; }
    }
}
