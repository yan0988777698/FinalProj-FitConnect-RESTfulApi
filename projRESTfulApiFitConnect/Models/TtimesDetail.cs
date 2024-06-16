﻿using System;
using System.Collections.Generic;

namespace projRESTfulApiFitConnect.Models;

public partial class TtimesDetail
{
    public int TimeId { get; set; }

    public TimeSpan TimeName { get; set; }

    public virtual ICollection<TGymTime> TGymTimes { get; set; } = new List<TGymTime>();

    public virtual ICollection<TclassSchedule> TclassScheduleCourseEndTimes { get; set; } = new List<TclassSchedule>();

    public virtual ICollection<TclassSchedule> TclassScheduleCourseStartTimes { get; set; } = new List<TclassSchedule>();
}
