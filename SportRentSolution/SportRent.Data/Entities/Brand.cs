using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class Brand
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
