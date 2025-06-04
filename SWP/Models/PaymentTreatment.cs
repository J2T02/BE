using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class PaymentTreatment
{
    public int PtId { get; set; }

    public int TpId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int? MethodId { get; set; }

    public int? StatusId { get; set; }

    public virtual MethodPayment? Method { get; set; }

    public virtual StatusPayment? Status { get; set; }

    public virtual TreatmentPlan Tp { get; set; } = null!;
}
