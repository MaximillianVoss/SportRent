using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class PaymentStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
