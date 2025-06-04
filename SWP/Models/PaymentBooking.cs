using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class PaymentBooking
{
    public int PbId { get; set; }

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int? MethodId { get; set; }

    public int? StatusId { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual MethodPayment? Method { get; set; }

    public virtual StatusPayment? Status { get; set; }
}
