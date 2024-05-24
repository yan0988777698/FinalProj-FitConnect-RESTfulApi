﻿using System;
using System.Collections.Generic;

namespace projRESTfulApiFitConnect.Models;

public partial class TfieldReserve
{
    public int FieldReserveId { get; set; }

    public int FieldId { get; set; }

    public int CoachId { get; set; }

    public bool PaymentStatus { get; set; }

    public bool? ReserveStatus { get; set; }

    public virtual TIdentity Coach { get; set; } = null!;

    public virtual Tfield Field { get; set; } = null!;
}
