using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class BioSample
{
    public int BsId { get; set; }

    public int? TpId { get; set; }

    public int? BtId { get; set; }

    public string? BsName { get; set; }

    public string? Status { get; set; }

    public string? Quality { get; set; }

    public DateOnly? CollectionDate { get; set; }

    public string? StorageLocation { get; set; }

    public string? Note { get; set; }

    public virtual BioType? Bt { get; set; }

    public virtual TreatmentPlan? Tp { get; set; }
}
