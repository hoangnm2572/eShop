namespace BusinessObjects;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? FullName { get; set; } = null!;

    public string Role { get; set; } = null!;

    public int? BranchId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual ICollection<InventoryTransfer> InventoryTransfers { get; set; } = new List<InventoryTransfer>();
}
