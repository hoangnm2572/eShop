using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class Product
{
    public int Id { get; set; }

    public string Barcode { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal SalePrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<InventoryLedger> InventoryLedgers { get; set; } = new List<InventoryLedger>();

    public virtual ICollection<InventoryTransferDetail> InventoryTransferDetails { get; set; } = new List<InventoryTransferDetail>();
}
