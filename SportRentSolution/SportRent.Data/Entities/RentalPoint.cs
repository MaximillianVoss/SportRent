using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class RentalPoint
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Phone { get; set; }

    public virtual ICollection<RentOrder> RentOrderIdRentalPointIssueNavigations { get; set; } = new List<RentOrder>();

    public virtual ICollection<RentOrder> RentOrderIdRentalPointReturnNavigations { get; set; } = new List<RentOrder>();

    public virtual ICollection<RentalPointEquipment> RentalPointEquipments { get; set; } = new List<RentalPointEquipment>();
}
