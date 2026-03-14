using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class RentalPointEquipment
{
    public int Id { get; set; }

    public int IdRentalPoint { get; set; }

    public int IdEquipment { get; set; }

    public int IdEquipmentCondition { get; set; }

    public int IdSize { get; set; }

    public int TotalQuantity { get; set; }

    public int AvailableQuantity { get; set; }

    public virtual EquipmentCondition IdEquipmentConditionNavigation { get; set; } = null!;

    public virtual Equipment IdEquipmentNavigation { get; set; } = null!;

    public virtual RentalPoint IdRentalPointNavigation { get; set; } = null!;

    public virtual EquipmentSize IdSizeNavigation { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
