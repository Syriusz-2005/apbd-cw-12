using System;
using System.Collections.Generic;

namespace apbd_cw_12;

public partial class ClientTrip
{
    public int IdClient { get; set; }

    public int IdTrip { get; set; }

    public DateTime RegisteredAt { get; set; }

    public DateTime? PaymentDate { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Trip IdTripNavigation { get; set; } = null!;
}
