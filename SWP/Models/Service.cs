using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class Service
{
    public int SerId { get; set; }

    public string? SerName { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
}
