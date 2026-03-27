using BusinessObjects;
using Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IInventoryRepository : IBaseRepository<Inventory>
    {
        Task<IEnumerable<Inventory>> GetInventoryWithDetailsAsync(int branchId);
        Task<IEnumerable<Inventory>> GetInventoryWithDetailsAsync();
        Task<Inventory?> GetInventoryByBranchAndProductAsync(int branchId, int productId);
        Task<(IEnumerable<Inventory> Items, int TotalCount)> GetPagedInventoryWithDetailsAsync(int? branchId, string? searchTerm, int? productGroupId, int? supplierId, int page, int pageSize, bool inStockOnly);
    }
}