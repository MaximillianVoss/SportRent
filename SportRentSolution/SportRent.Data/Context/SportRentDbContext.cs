using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SportRent.Data.Entities;

namespace SportRent.Data.Context;

public partial class SportRentDbContext : DbContext
{
    public SportRentDbContext(DbContextOptions<SportRentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<EquipmentCondition> EquipmentConditions { get; set; }

    public virtual DbSet<EquipmentPhoto> EquipmentPhotos { get; set; }

    public virtual DbSet<EquipmentRate> EquipmentRates { get; set; }

    public virtual DbSet<EquipmentSize> EquipmentSizes { get; set; }

    public virtual DbSet<EquipmentType> EquipmentTypes { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<PaymentStatus> PaymentStatuses { get; set; }

    public virtual DbSet<RentOrder> RentOrders { get; set; }

    public virtual DbSet<RentalPoint> RentalPoints { get; set; }

    public virtual DbSet<RentalPointEquipment> RentalPointEquipments { get; set; }

    public virtual DbSet<RentalType> RentalTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("brands");

            entity.HasIndex(e => e.Title, "IX_brands_title").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");

            entity.HasIndex(e => e.Title, "IX_categories_title").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.ToTable("equipment");

            entity.HasIndex(e => e.IdBrand, "idx_equipment_brand");

            entity.HasIndex(e => e.IdCategory, "idx_equipment_category");

            entity.HasIndex(e => e.IdType, "idx_equipment_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IdBrand).HasColumnName("idBrand");
            entity.Property(e => e.IdCategory).HasColumnName("idCategory");
            entity.Property(e => e.IdType).HasColumnName("idType");
            entity.Property(e => e.Model).HasColumnName("model");
            entity.Property(e => e.Title).HasColumnName("title");

            entity.HasOne(d => d.IdBrandNavigation).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.IdBrand)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdCategoryNavigation).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.IdCategory)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdTypeNavigation).WithMany(p => p.Equipment)
                .HasForeignKey(d => d.IdType)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EquipmentCondition>(entity =>
        {
            entity.ToTable("equipmentConditions");

            entity.HasIndex(e => e.Title, "IX_equipmentConditions_title").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<EquipmentPhoto>(entity =>
        {
            entity.ToTable("equipmentPhotos");

            entity.HasIndex(e => new { e.IdEquipment, e.IdImage }, "IX_equipmentPhotos_idEquipment_idImage").IsUnique();

            entity.HasIndex(e => e.IdEquipment, "idx_equipmentPhotos_equipment");

            entity.HasIndex(e => e.IdImage, "idx_equipmentPhotos_image");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdEquipment).HasColumnName("idEquipment");
            entity.Property(e => e.IdImage).HasColumnName("idImage");

            entity.HasOne(d => d.IdEquipmentNavigation).WithMany(p => p.EquipmentPhotos).HasForeignKey(d => d.IdEquipment);

            entity.HasOne(d => d.IdImageNavigation).WithMany(p => p.EquipmentPhotos).HasForeignKey(d => d.IdImage);
        });

        modelBuilder.Entity<EquipmentRate>(entity =>
        {
            entity.ToTable("equipmentRates");

            entity.HasIndex(e => new { e.IdEquipment, e.IdRentalType }, "IX_equipmentRates_idEquipment_idRentalType").IsUnique();

            entity.HasIndex(e => e.IdEquipment, "idx_equipmentRates_equipment");

            entity.HasIndex(e => e.IdRentalType, "idx_equipmentRates_rentalType");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Deposit).HasColumnName("deposit");
            entity.Property(e => e.IdEquipment).HasColumnName("idEquipment");
            entity.Property(e => e.IdRentalType).HasColumnName("idRentalType");
            entity.Property(e => e.Price).HasColumnName("price");

            entity.HasOne(d => d.IdEquipmentNavigation).WithMany(p => p.EquipmentRates).HasForeignKey(d => d.IdEquipment);

            entity.HasOne(d => d.IdRentalTypeNavigation).WithMany(p => p.EquipmentRates)
                .HasForeignKey(d => d.IdRentalType)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EquipmentSize>(entity =>
        {
            entity.ToTable("equipmentSizes");

            entity.HasIndex(e => e.Title, "IX_equipmentSizes_title").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<EquipmentType>(entity =>
        {
            entity.ToTable("equipmentTypes");

            entity.HasIndex(e => e.Title, "IX_equipmentTypes_title").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("images");

            entity.HasIndex(e => e.Url, "IX_images_url").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("dateCreated");
            entity.Property(e => e.Url).HasColumnName("url");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("orderItems");

            entity.HasIndex(e => e.IdOrder, "idx_orderItems_order");

            entity.HasIndex(e => e.IdRentalPointEquipment, "idx_orderItems_rentalPointEquipment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.IdOrder).HasColumnName("idOrder");
            entity.Property(e => e.IdRentalPointEquipment).HasColumnName("idRentalPointEquipment");
            entity.Property(e => e.PricePerUnit).HasColumnName("pricePerUnit");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(1)
                .HasColumnName("quantity");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.OrderItems).HasForeignKey(d => d.IdOrder);

            entity.HasOne(d => d.IdRentalPointEquipmentNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.IdRentalPointEquipment)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.ToTable("orderStatuses");

            entity.HasIndex(e => e.Title, "IX_orderStatuses_title").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");

            entity.HasIndex(e => e.IdPaymentMethod, "idx_payments_method");

            entity.HasIndex(e => e.IdOrder, "idx_payments_order");

            entity.HasIndex(e => e.IdStatus, "idx_payments_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("dateCreated");
            entity.Property(e => e.IdOrder).HasColumnName("idOrder");
            entity.Property(e => e.IdPaymentMethod).HasColumnName("idPaymentMethod");
            entity.Property(e => e.IdStatus).HasColumnName("idStatus");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.Payments).HasForeignKey(d => d.IdOrder);

            entity.HasOne(d => d.IdPaymentMethodNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.IdPaymentMethod)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdStatusNavigation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.IdStatus)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.ToTable("paymentMethods");

            entity.HasIndex(e => e.Title, "IX_paymentMethods_title").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<PaymentStatus>(entity =>
        {
            entity.ToTable("paymentStatuses");

            entity.HasIndex(e => e.Title, "IX_paymentStatuses_title").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<RentOrder>(entity =>
        {
            entity.ToTable("rentOrders");

            entity.HasIndex(e => e.IdRentalPointIssue, "idx_rentOrders_issuePoint");

            entity.HasIndex(e => e.IdRentalPointReturn, "idx_rentOrders_returnPoint");

            entity.HasIndex(e => e.IdStatus, "idx_rentOrders_status");

            entity.HasIndex(e => e.IdUser, "idx_rentOrders_user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("dateCreated");
            entity.Property(e => e.DateEnd).HasColumnName("dateEnd");
            entity.Property(e => e.DateStart).HasColumnName("dateStart");
            entity.Property(e => e.DepositAmount).HasColumnName("depositAmount");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IdRentalPointIssue).HasColumnName("idRentalPointIssue");
            entity.Property(e => e.IdRentalPointReturn).HasColumnName("idRentalPointReturn");
            entity.Property(e => e.IdStatus).HasColumnName("idStatus");
            entity.Property(e => e.IdUser).HasColumnName("idUser");

            entity.HasOne(d => d.IdRentalPointIssueNavigation).WithMany(p => p.RentOrderIdRentalPointIssueNavigations)
                .HasForeignKey(d => d.IdRentalPointIssue)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdRentalPointReturnNavigation).WithMany(p => p.RentOrderIdRentalPointReturnNavigations)
                .HasForeignKey(d => d.IdRentalPointReturn)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdStatusNavigation).WithMany(p => p.RentOrders)
                .HasForeignKey(d => d.IdStatus)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.RentOrders)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RentalPoint>(entity =>
        {
            entity.ToTable("rentalPoints");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Phone).HasColumnName("phone");
        });

        modelBuilder.Entity<RentalPointEquipment>(entity =>
        {
            entity.ToTable("rentalPointEquipment");

            entity.HasIndex(e => new { e.IdRentalPoint, e.IdEquipment, e.IdEquipmentCondition, e.IdSize }, "IX_rentalPointEquipment_idRentalPoint_idEquipment_idEquipmentCondition_idSize").IsUnique();

            entity.HasIndex(e => e.IdEquipmentCondition, "idx_rentalPointEquipment_condition");

            entity.HasIndex(e => e.IdEquipment, "idx_rentalPointEquipment_equipment");

            entity.HasIndex(e => e.IdRentalPoint, "idx_rentalPointEquipment_point");

            entity.HasIndex(e => e.IdSize, "idx_rentalPointEquipment_size");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvailableQuantity).HasColumnName("availableQuantity");
            entity.Property(e => e.IdEquipment).HasColumnName("idEquipment");
            entity.Property(e => e.IdEquipmentCondition).HasColumnName("idEquipmentCondition");
            entity.Property(e => e.IdRentalPoint).HasColumnName("idRentalPoint");
            entity.Property(e => e.IdSize).HasColumnName("idSize");
            entity.Property(e => e.TotalQuantity).HasColumnName("totalQuantity");

            entity.HasOne(d => d.IdEquipmentNavigation).WithMany(p => p.RentalPointEquipments)
                .HasForeignKey(d => d.IdEquipment)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdEquipmentConditionNavigation).WithMany(p => p.RentalPointEquipments)
                .HasForeignKey(d => d.IdEquipmentCondition)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdRentalPointNavigation).WithMany(p => p.RentalPointEquipments)
                .HasForeignKey(d => d.IdRentalPoint)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdSizeNavigation).WithMany(p => p.RentalPointEquipments)
                .HasForeignKey(d => d.IdSize)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RentalType>(entity =>
        {
            entity.ToTable("rentalTypes");

            entity.HasIndex(e => e.Code, "IX_rentalTypes_code").IsUnique();

            entity.HasIndex(e => e.Title, "IX_rentalTypes_title").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.UnitHours).HasColumnName("unitHours");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("roles");

            entity.HasIndex(e => e.Title, "IX_roles_title").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "IX_users_email").IsUnique();

            entity.HasIndex(e => e.Phone, "IX_users_phone").IsUnique();

            entity.HasIndex(e => e.IdRole, "idx_users_role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("dateCreated");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FirstName).HasColumnName("firstName");
            entity.Property(e => e.IdRole).HasColumnName("idRole");
            entity.Property(e => e.LastName).HasColumnName("lastName");
            entity.Property(e => e.PasswordHash).HasColumnName("passwordHash");
            entity.Property(e => e.Phone).HasColumnName("phone");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
