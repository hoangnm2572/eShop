using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Repositories.dbContext;

public partial class eShopDbContext : DbContext
{
    public eShopDbContext()
    {
    }

    public eShopDbContext(DbContextOptions<eShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryLedger> InventoryLedgers { get; set; }

    public virtual DbSet<InventoryTransfer> InventoryTransfers { get; set; }

    public virtual DbSet<InventoryTransferDetail> InventoryTransferDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public DbSet<Supplier> Suppliers { get; set; }

    public DbSet<ProductGroup> ProductGroups { get; set; }

    public DbSet<UnitConversion> UnitConversions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("MyCnn");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Branches__3214EC079DC2151D");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.BranchType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => new { e.BranchId, e.ProductId }).HasName("PK__Inventor__6A28E3A9924A1FE8");

            entity.HasOne(d => d.Branch).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventori__Branc__5812160E");

            entity.HasOne(d => d.Product).WithMany(p => p.Inventories)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventori__Produ__59063A47");
        });

        modelBuilder.Entity<InventoryLedger>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Inventor__3214EC07F66B68EE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ReferenceCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TransactionType)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Branch).WithMany(p => p.InventoryLedgers)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Branc__6754599E");

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryLedgers)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__68487DD7");

            entity.HasOne(d => d.Creator)
                .WithMany()
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryLedger_CreatedBy_User");
        });

        modelBuilder.Entity<InventoryTransfer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Inventor__3214EC076396C06C");

            entity.HasIndex(e => e.TransferCode, "UQ__Inventor__CE99A4C53C52F480").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TransferCode)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InventoryTransfers)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Creat__5FB337D6");

            entity.HasOne(d => d.FromBranch).WithMany(p => p.InventoryTransferFromBranches)
                .HasForeignKey(d => d.FromBranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__FromB__5DCAEF64");

            entity.HasOne(d => d.ToBranch).WithMany(p => p.InventoryTransferToBranches)
                .HasForeignKey(d => d.ToBranchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__ToBra__5EBF139D");

            entity.HasOne(d => d.ApprovedByNavigation)
                .WithMany()
                .HasForeignKey(d => d.ApprovedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryTransfer_ApprovedBy_User");
        });

        modelBuilder.Entity<InventoryTransferDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Inventor__3214EC07A5B91520");

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryTransferDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Produ__6477ECF3");

            entity.HasOne(d => d.Transfer).WithMany(p => p.InventoryTransferDetails)
                .HasForeignKey(d => d.TransferId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Trans__6383C8BA");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07F2F5B48B");

            entity.HasIndex(e => e.Barcode, "UQ__Products__177800D3499D0183")
                  .IsUnique()
                  .HasFilter("[Barcode] IS NOT NULL");

            entity.HasIndex(e => e.Sku, "UQ__Products__CA1ECF0D4BCAAD3C").IsUnique();

            entity.Property(e => e.Barcode).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Sku).HasMaxLength(50).IsUnicode(false).HasColumnName("SKU");
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();

            entity.Property(e => e.BaseUnit).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.ShowOnPos).HasDefaultValue(true);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Supplier)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId)
                .HasConstraintName("FK_Products_Suppliers")
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.ProductGroup)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductGroupId)
                .HasConstraintName("FK_Products_ProductGroups")
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07C5ABB65B");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E43150DA93").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.FullName)
                .HasMaxLength(150)  
                .IsRequired() 
                .IsUnicode(true); 

            entity.HasOne(d => d.Branch).WithMany(p => p.Users)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__Users__BranchId__4E88ABD4");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Suppliers");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.ContactName)
                .HasMaxLength(100);

            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Address)
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        });

        modelBuilder.Entity<ProductGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ProductGroups");
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<UnitConversion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UnitConversions");

            entity.Property(e => e.UnitName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SalePrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Product)
                .WithMany(p => p.UnitConversions)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_UnitConversions_Products")
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
