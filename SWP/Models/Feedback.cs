using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class Feedback
{
    public int FbId { get; set; }

    public int? TpId { get; set; }

    public int? DocId { get; set; }

    public int? Star { get; set; }

    public string? Content { get; set; }

    public virtual Doctor? Doc { get; set; }

    public virtual TreatmentPlan? Tp { get; set; }
}
