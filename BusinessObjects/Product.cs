namespace BusinessObjects;

public partial class Product
{
    public int Id { get; set; }

    public string? Barcode { get; set; }

    public string Sku { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string BaseUnit { get; set; } = null!;

    public decimal PurchasePrice { get; set; }

    public decimal SalePrice { get; set; }

    public bool IsActive { get; set; } = true;

    public bool ShowOnPos { get; set; } = true;

    public DateTime? CreatedAt { get; set; }

    public int? ProductGroupId { get; set; }
    public virtual ProductGroup? ProductGroup { get; set; }

    public int? SupplierId { get; set; }
    public virtual Supplier? Supplier { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<InventoryLedger> InventoryLedgers { get; set; } = new List<InventoryLedger>();

    public virtual ICollection<InventoryTransferDetail> InventoryTransferDetails { get; set; } = new List<InventoryTransferDetail>();

    public virtual ICollection<UnitConversion> UnitConversions { get; set; } = new List<UnitConversion>();
}