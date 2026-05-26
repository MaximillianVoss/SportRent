using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class EquipmentRate
{
    public int Id { get; set; }

    public int IdEquipment { get; set; }

    public int IdRentalType { get; set; }

    public int Price { get; set; }

    public int Deposit { get; set; }

    public virtual Equipment IdEquipmentNavigation { get; set; } = null!;

    public virtual RentalType IdRentalTypeNavigation { get; set; } = null!;
}
