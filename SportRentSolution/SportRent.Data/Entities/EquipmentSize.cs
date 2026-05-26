using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class EquipmentSize
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<RentalPointEquipment> RentalPointEquipments { get; set; } = new List<RentalPointEquipment>();
}
