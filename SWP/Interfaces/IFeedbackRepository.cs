﻿using SWP.Models;

namespace SWP.Interfaces
{
    public interface IFeedbackRepository
    {
        Task<IEnumerable<Feedback>> GetFeedbacksByDoctorIdAsync(int doctorId);
        Task<IEnumerable<Feedback>> GetFeedbacksByTreatmentPlanIdAsync(int treatmentPlanId);
        Task<Feedback?> GetFeedbackById(int id);
        Task<Feedback?> PostFeedback(Feedback request);
    }
}
