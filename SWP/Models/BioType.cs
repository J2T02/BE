using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class BioType
{
    public int BtId { get; set; }

    public string? BtName { get; set; }

    public virtual ICollection<BioSample> BioSamples { get; set; } = new List<BioSample>();
}
