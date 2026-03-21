using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class InventoryTransfer
{
    public int Id { get; set; }

    public string TransferCode { get; set; } = null!;

    public int FromBranchId { get; set; }

    public int ToBranchId { get; set; }

    public string Status { get; set; } = null!;

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? Note { get; set; }

    public virtual User? CreatedByNavigation { get; set; }

    public virtual Branch FromBranch { get; set; } = null!;

    public virtual ICollection<InventoryTransferDetail> InventoryTransferDetails { get; set; } = new List<InventoryTransferDetail>();

    public virtual Branch ToBranch { get; set; } = null!;
}
