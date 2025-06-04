using System;
using System.Collections.Generic;

namespace SWP.Models;

public partial class SlotSchedule
{
    public int SlotId { get; set; }

    public TimeOnly SlotStart { get; set; }

    public TimeOnly SlotEnd { get; set; }

    public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();
}
