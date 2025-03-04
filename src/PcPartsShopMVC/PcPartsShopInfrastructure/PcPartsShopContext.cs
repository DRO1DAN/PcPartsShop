using System;
using System.Collections.Generic;
using PcPartsShopDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace PcPartsShopInfrastructure;

public partial class PcPartsShopContext : DbContext
{
    public PcPartsShopContext()
    {
    }

    public PcPartsShopContext(DbContextOptions<PcPartsShopContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<ComputerCase> Cases { get; set; }

    public virtual DbSet<Cpu> Cpus { get; set; }

    public virtual DbSet<Gpu> Gpus { get; set; }

    public virtual DbSet<Motherboard> Motherboards { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Psu> Psus { get; set; }

    public virtual DbSet<Ram> Rams { get; set; }

    public virtual DbSet<Storage> Storages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DROIDAN\\SQLEXPRESS; Database=PcPartsShop; Trusted_Connection=True; TrustServerCertificate=True; ");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brand__3213E83F13F01884");

            entity.ToTable("Brand");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart__3213E83FFC4B1433");

            entity.ToTable("Cart");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Cart_User");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CartItem__3213E83FF7B13F4B");

            entity.ToTable("CartItem");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItem_Cart");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CartItem_Product");
        });

        modelBuilder.Entity<ComputerCase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Case__3213E83F66BAE731");

            entity.ToTable("Case");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FormFactor)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.Cases)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Case_Product");
        });

        modelBuilder.Entity<Cpu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CPU__3213E83FBBA05A36");

            entity.ToTable("CPU");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BaseClock).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.BoostClock).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Generation)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Series)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Socket)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SupportedMemoryType)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Tdp).HasColumnName("TDP");

            entity.HasOne(d => d.Product).WithMany(p => p.Cpus)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CPU_Product");
        });

        modelBuilder.Entity<Gpu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GPU__3213E83FC8806239");

            entity.ToTable("GPU");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BaseClock).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.BoostClock).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Generation)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MemoryType)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PcieVersion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PCIeVersion");
            entity.Property(e => e.Series)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Tdp).HasColumnName("TDP");

            entity.HasOne(d => d.Product).WithMany(p => p.Gpus)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GPU_Product");
        });

        modelBuilder.Entity<Motherboard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Motherbo__3213E83FE683CE42");

            entity.ToTable("Motherboard");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Chipset)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FormFactor)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MemoryType)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PcieVersion)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PCIeVersion");
            entity.Property(e => e.Socket)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.Motherboards)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Motherboard_Product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3213E83F783EF790");

            entity.ToTable("Product");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Brand");
        });

        modelBuilder.Entity<Psu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PSU__3213E83F5920E5E6");

            entity.ToTable("PSU");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Efficiency)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.Psus)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PSU_Product");
        });

        modelBuilder.Entity<Ram>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RAM__3213E83FA2CC7E6F");

            entity.ToTable("RAM");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.Rams)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RAM_Product");
        });

        modelBuilder.Entity<Storage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Storage__3213E83F77427008");

            entity.ToTable("Storage");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Interface)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.MemoryType)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.Storages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Storage_Product");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3213E83F9A81373E");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D105342D903C5C").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
