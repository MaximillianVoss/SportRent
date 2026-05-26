using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class EquipmentPhoto
{
    public int Id { get; set; }

    public int IdEquipment { get; set; }

    public int IdImage { get; set; }

    public virtual Equipment IdEquipmentNavigation { get; set; } = null!;

    public virtual Image IdImageNavigation { get; set; } = null!;
}
