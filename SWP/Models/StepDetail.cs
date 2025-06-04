using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class StepDetail
{
    public int SdId { get; set; }

    public int? TpId { get; set; }

    public int? TsId { get; set; }

    public string? StepName { get; set; }

    public string? Note { get; set; }

    public string? Status { get; set; }

    public DateOnly? PlanDate { get; set; }

    public DateOnly? DoneDate { get; set; }

    public string? DrugName { get; set; }

    public string? Dosage { get; set; }

    public virtual TreatmentPlan? Tp { get; set; }

    public virtual TreatmentStep? Ts { get; set; }
}
