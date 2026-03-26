using BusinessObjects;
using Repositories.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IInventoryTransferRepository : IBaseRepository<InventoryTransfer>
    {
        Task<InventoryTransfer?> GetTransferWithDetailsAsync(int transferId);
        Task<(IEnumerable<InventoryTransfer> Items, int TotalCount)> GetPagedTransfersWithDetailsAsync(
            int? branchId, string? searchTerm, string? status, DateTime? startDate, DateTime? endDate, int page, int pageSize);
        Task<InventoryTransfer?> GetByTransferCodeAsync(string transferCode);
        Task<IEnumerable<InventoryTransfer>> GetTransfersByCodesAsync(IEnumerable<string> codes);
    }
}