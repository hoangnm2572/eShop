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
    public class InventoryTransferRepository : BaseRepository<InventoryTransfer>, IInventoryTransferRepository
    {
        public InventoryTransferRepository(eShopDbContext context) : base(context) { }

        public async Task<InventoryTransfer?> GetTransferWithDetailsAsync(int transferId)
        {
            return await _context.InventoryTransfers
                .AsNoTracking()
                .Include(t => t.InventoryTransferDetails)
                .FirstOrDefaultAsync(t => t.Id == transferId);
        }

        public async Task<(IEnumerable<InventoryTransfer> Items, int TotalCount)> GetPagedTransfersWithDetailsAsync(
            int? branchId, string? searchTerm, string? status, DateTime? startDate, DateTime? endDate, int page, int pageSize, bool isRequestOnly = false)
        {
            var query = _context.InventoryTransfers.AsQueryable();

            if (isRequestOnly)
            {
                query = query.Where(t => t.TransferCode != null &&
                                         t.TransferCode.Trim().ToUpper().StartsWith("REQ"));
            }

            Console.WriteLine(query.ToQueryString());

            if (branchId.HasValue)
                query = query.Where(t => t.FromBranchId == branchId.Value || t.ToBranchId == branchId.Value);

            if (!string.IsNullOrEmpty(status) && status != "ALL")
                query = query.Where(t => t.Status == status);

            if (startDate.HasValue)
            {
                var startOfDay = startDate.Value.Date;
                query = query.Where(t => t.CreatedAt >= startOfDay);
            }

            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(t => t.CreatedAt <= endOfDay);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                query = query.Where(t =>
                    (t.TransferCode != null && t.TransferCode.ToLower().Contains(term)) ||
                    t.InventoryTransferDetails.Any(d =>
                        (d.Product.Name != null && d.Product.Name.ToLower().Contains(term)) ||
                        (d.Product.Sku != null && d.Product.Sku.ToLower().Contains(term))
                    )
                );
            }

            int totalCount = await query.CountAsync();
            var items = await query
                .AsNoTracking()
                .Include(t => t.FromBranch)
                .Include(t => t.ToBranch)
                .Include(t => t.CreatedByNavigation)
                .Include(t => t.ApprovedByNavigation)
                .Include(t => t.InventoryTransferDetails)
                    .ThenInclude(d => d.Product)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<InventoryTransfer?> GetByTransferCodeAsync(string transferCode)
        {
            return await _context.InventoryTransfers
                .AsNoTracking()
                .Include(t => t.ApprovedByNavigation)
                .FirstOrDefaultAsync(t => t.TransferCode == transferCode);
        }

        public async Task<IEnumerable<InventoryTransfer>> GetTransfersByCodesAsync(IEnumerable<string> codes)
        {
            if (codes == null || !codes.Any()) return new List<InventoryTransfer>();

            return await _context.InventoryTransfers
                .AsNoTracking()
                .Include(t => t.FromBranch)
                .Include(t => t.ToBranch)
                .Include(t => t.ApprovedByNavigation)
                .Where(t => codes.Contains(t.TransferCode))
                .ToListAsync();
        }
    }
}