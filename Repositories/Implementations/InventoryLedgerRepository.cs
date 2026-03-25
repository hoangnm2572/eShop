using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.dbContext;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Implementations
{
    public class InventoryLedgerRepository : BaseRepository<InventoryLedger>, IInventoryLedgerRepository
    {
        public InventoryLedgerRepository(eShopDbContext context) : base(context)
        {
        }

        public IEnumerable<InventoryLedger> GetLedgersWithDetails(int? branchId = null)
        {
            var query = _context.InventoryLedgers
                .Include(l => l.Product)
                .Include(l => l.Branch)
                .Include(l => l.Creator)
                .AsQueryable();

            if (branchId.HasValue)
            {
                query = query.Where(l => l.BranchId == branchId.Value);
            }

            return query.OrderByDescending(l => l.CreatedAt).ToList();
        }
    }
}
