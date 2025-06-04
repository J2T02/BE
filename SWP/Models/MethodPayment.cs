using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class MethodPayment
{
    public int MethodId { get; set; }

    public string MethodName { get; set; } = null!;

    public virtual ICollection<PaymentBooking> PaymentBookings { get; set; } = new List<PaymentBooking>();

    public virtual ICollection<PaymentTreatment> PaymentTreatments { get; set; } = new List<PaymentTreatment>();
}
