using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class Customer
{
    public int CusId { get; set; }

    public int? AccId { get; set; }

    public string? HusName { get; set; }

    public string? WifeName { get; set; }

    public DateOnly? HusYob { get; set; }

    public DateOnly? WifeYob { get; set; }

    public string? Phone { get; set; }

    public string? Mail { get; set; }

    public virtual Account? Acc { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();

    public virtual ICollection<TreatmentPlan> TreatmentPlans { get; set; } = new List<TreatmentPlan>();
}
