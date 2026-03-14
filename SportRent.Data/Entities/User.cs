using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class User
{
    public int Id { get; set; }

    public int IdRole { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public virtual Role IdRoleNavigation { get; set; } = null!;

    public virtual ICollection<RentOrder> RentOrders { get; set; } = new List<RentOrder>();
}
