using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class RentalType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int UnitHours { get; set; }

    public virtual ICollection<EquipmentRate> EquipmentRates { get; set; } = new List<EquipmentRate>();
}
