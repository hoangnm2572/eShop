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
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(eShopDbContext context) : base(context) { }

        public IEnumerable<Inventory> GetInventoryWithDetails(int branchId)
        {
            return _context.Inventories
                .Include(i => i.Product)
                    .ThenInclude(p => p.UnitConversions)
                .Include(i => i.Product)
                    .ThenInclude(p => p.ProductGroup)
                .Include(i => i.Product)
                    .ThenInclude(p => p.Supplier)
                .Where(i => i.BranchId == branchId)
                .ToList();
        }
    }
}
