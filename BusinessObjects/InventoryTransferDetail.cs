using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class InventoryTransferDetail
{
    public long Id { get; set; }

    public int TransferId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual InventoryTransfer Transfer { get; set; } = null!;
}
