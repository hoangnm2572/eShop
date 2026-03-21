using System;
using System.Collections.Generic;

namespace BussinessObjects;

public partial class Inventory
{
    public int BranchId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
