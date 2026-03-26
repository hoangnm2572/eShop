using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.dbContext;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class InventoryLedgerRepository : BaseRepository<InventoryLedger>, IInventoryLedgerRepository
    {
        public InventoryLedgerRepository(eShopDbContext context) : base(context) { }

        public async Task<(IEnumerable<InventoryLedger> Items, int TotalCount)> GetPagedLedgersWithDetailsAsync(
            int? branchId, string? searchTerm, string? transactionType, DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            var query = _context.InventoryLedgers
                .AsNoTracking()
                .Include(l => l.Product)
                .Include(l => l.Branch)
                .Include(l => l.Creator)
                .AsQueryable();

            if (branchId.HasValue)
                query = query.Where(l => l.BranchId == branchId.Value);

            if (!string.IsNullOrEmpty(transactionType) && transactionType != "ALL")
                query = query.Where(l => l.TransactionType == transactionType);

            if (startDate.HasValue)
            {
                var startOfDay = startDate.Value.Date;
                query = query.Where(l => l.CreatedAt >= startOfDay);
            }

            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(l => l.CreatedAt <= endOfDay);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                query = query.Where(l =>
                    (l.ReferenceCode != null && l.ReferenceCode.ToLower().Contains(term)) ||
                    (l.Product.Name != null && l.Product.Name.ToLower().Contains(term)) ||
                    (l.Product.Sku != null && l.Product.Sku.ToLower().Contains(term))
                );
            }

            int totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(l => l.CreatedAt)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(IEnumerable<InventoryLedger> Items, int TotalGroups)> GetPagedGroupedLedgersAsync(
            int? branchId, string? searchTerm, string? transactionType, DateTime? startDate, DateTime? endDate, int page, int pageSize)
        {
            var query = _context.InventoryLedgers
                .AsNoTracking()
                .Where(l => !string.IsNullOrEmpty(l.ReferenceCode));

            if (branchId.HasValue)
                query = query.Where(l => l.BranchId == branchId.Value);

            if (!string.IsNullOrEmpty(transactionType) && transactionType != "ALL")
                query = query.Where(l => l.TransactionType == transactionType);

            if (startDate.HasValue)
            {
                var startOfDay = startDate.Value.Date;
                query = query.Where(l => l.CreatedAt >= startOfDay);
            }

            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(l => l.CreatedAt <= endOfDay);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                query = query.Where(l =>
                    (l.ReferenceCode != null && l.ReferenceCode.ToLower().Contains(term)) ||
                    (l.Product.Name != null && l.Product.Name.ToLower().Contains(term)) ||
                    (l.Product.Sku != null && l.Product.Sku.ToLower().Contains(term))
                );
            }

            int totalGroups = await query.Select(l => l.ReferenceCode).Distinct().CountAsync();

            var pagedRefCodes = await query
                .GroupBy(x => x.ReferenceCode)
                .Select(g => new { ReferenceCode = g.Key, MaxCreatedAt = g.Max(x => x.CreatedAt) })
                .OrderByDescending(x => x.MaxCreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.ReferenceCode)
                .ToListAsync();

            if (!pagedRefCodes.Any())
            {
                return (new List<InventoryLedger>(), totalGroups);
            }

            var items = await query
                .Include(l => l.Product)
                .Include(l => l.Branch)
                .Include(l => l.Creator)
                .Where(l => pagedRefCodes.Contains(l.ReferenceCode))
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();

            return (items, totalGroups);
        }
    }
}