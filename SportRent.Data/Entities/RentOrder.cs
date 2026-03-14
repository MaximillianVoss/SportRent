using System;
using System.Collections.Generic;

namespace SportRent.Data.Entities;

public partial class RentOrder
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public int IdStatus { get; set; }

    public int IdRentalPointIssue { get; set; }

    public int? IdRentalPointReturn { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateStart { get; set; }

    public DateTime DateEnd { get; set; }

    public int Amount { get; set; }

    public int DepositAmount { get; set; }

    public string? Description { get; set; }

    public virtual RentalPoint IdRentalPointIssueNavigation { get; set; } = null!;

    public virtual RentalPoint? IdRentalPointReturnNavigation { get; set; }

    public virtual OrderStatus IdStatusNavigation { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
