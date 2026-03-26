using BusinessObjects;
using Repositories.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IInventoryLedgerRepository : IBaseRepository<InventoryLedger>
    {
        Task<(IEnumerable<InventoryLedger> Items, int TotalCount)> GetPagedLedgersWithDetailsAsync(
            int? branchId, string? searchTerm, string? transactionType, DateTime? startDate, DateTime? endDate, int page, int pageSize);

        Task<(IEnumerable<InventoryLedger> Items, int TotalGroups)> GetPagedGroupedLedgersAsync(
            int? branchId, string? searchTerm, string? transactionType, DateTime? startDate, DateTime? endDate, int page, int pageSize);
    }
}