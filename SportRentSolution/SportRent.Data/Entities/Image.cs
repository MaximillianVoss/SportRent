using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class Image
{
    public int Id { get; set; }

    public string Url { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public virtual ICollection<EquipmentPhoto> EquipmentPhotos { get; set; } = new List<EquipmentPhoto>();
}
