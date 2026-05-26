using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class Equipment
{
    public int Id { get; set; }

    public int IdCategory { get; set; }

    public int IdBrand { get; set; }

    public int IdType { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Model { get; set; }

    public virtual ICollection<EquipmentPhoto> EquipmentPhotos { get; set; } = new List<EquipmentPhoto>();

    public virtual ICollection<EquipmentRate> EquipmentRates { get; set; } = new List<EquipmentRate>();

    public virtual Brand IdBrandNavigation { get; set; } = null!;

    public virtual Category IdCategoryNavigation { get; set; } = null!;

    public virtual EquipmentType IdTypeNavigation { get; set; } = null!;

    public virtual ICollection<RentalPointEquipment> RentalPointEquipments { get; set; } = new List<RentalPointEquipment>();
}
