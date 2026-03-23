namespace BusinessObjects;

public partial class Branch
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string BranchType { get; set; } = null!;

    public string? Address { get; set; }

    public bool? IsActive { get; set; } = true;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<InventoryLedger> InventoryLedgers { get; set; } = new List<InventoryLedger>();

    public virtual ICollection<InventoryTransfer> InventoryTransferFromBranches { get; set; } = new List<InventoryTransfer>();

    public virtual ICollection<InventoryTransfer> InventoryTransferToBranches { get; set; } = new List<InventoryTransfer>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
