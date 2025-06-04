using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class TreatmentPlan
{
    public int TpId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int? SerId { get; set; }

    public int? CurrentStepId { get; set; }

    public int? CusId { get; set; }

    public int? DocId { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<BioSample> BioSamples { get; set; } = new List<BioSample>();

    public virtual TreatmentStep? CurrentStep { get; set; }

    public virtual Customer? Cus { get; set; }

    public virtual Doctor? Doc { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<PaymentTreatment> PaymentTreatments { get; set; } = new List<PaymentTreatment>();

    public virtual Service? Ser { get; set; }

    public virtual ICollection<StepDetail> StepDetails { get; set; } = new List<StepDetail>();
}
