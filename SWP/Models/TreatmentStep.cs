using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class TreatmentStep
{
    public int TsId { get; set; }

    public string? StepName { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<StepDetail> StepDetails { get; set; } = new List<StepDetail>();

    public virtual ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
}
