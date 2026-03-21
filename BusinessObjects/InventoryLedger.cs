using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class InventoryLedger
{
    public long Id { get; set; }

    public int BranchId { get; set; }

    public int ProductId { get; set; }

    public string TransactionType { get; set; } = null!;

    public int QuantityChange { get; set; }

    public string? ReferenceCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
