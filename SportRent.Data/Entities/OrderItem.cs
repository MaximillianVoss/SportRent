using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class OrderItem
{
    public int Id { get; set; }

    public int IdOrder { get; set; }

    public int IdRentalPointEquipment { get; set; }

    public int Quantity { get; set; }

    public int PricePerUnit { get; set; }

    public int Amount { get; set; }

    public virtual RentOrder IdOrderNavigation { get; set; } = null!;

    public virtual RentalPointEquipment IdRentalPointEquipmentNavigation { get; set; } = null!;
}
