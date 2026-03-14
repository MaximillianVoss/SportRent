using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public int IdOrder { get; set; }

    public int IdPaymentMethod { get; set; }

    public int IdStatus { get; set; }

    public DateTime DateCreated { get; set; }

    public int Amount { get; set; }

    public virtual RentOrder IdOrderNavigation { get; set; } = null!;

    public virtual PaymentMethod IdPaymentMethodNavigation { get; set; } = null!;

    public virtual PaymentStatus IdStatusNavigation { get; set; } = null!;
}
