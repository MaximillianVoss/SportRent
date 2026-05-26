using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class OrderStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<RentOrder> RentOrders { get; set; } = new List<RentOrder>();
}
