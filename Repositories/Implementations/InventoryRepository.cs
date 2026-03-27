using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.dbContext;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(eShopDbContext context) : base(context) { }

        public async Task<IEnumerable<Inventory>> GetInventoryWithDetailsAsync(int branchId)
        {
            return await _context.Inventories
                .AsNoTracking()
                .Include(i => i.Product).ThenInclude(p => p.UnitConversions)
                .Include(i => i.Product).ThenInclude(p => p.ProductGroup)
                .Include(i => i.Product).ThenInclude(p => p.Supplier)
                .Where(i => i.BranchId == branchId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetInventoryWithDetailsAsync()
        {
            return await _context.Inventories
                .AsNoTracking()
                .Include(i => i.Product).ThenInclude(p => p.UnitConversions)
                .Include(i => i.Product).ThenInclude(p => p.ProductGroup)
                .Include(i => i.Product).ThenInclude(p => p.Supplier)
                .ToListAsync();
        }

        public async Task<Inventory?> GetInventoryByBranchAndProductAsync(int branchId, int productId)
        {
            return await _context.Inventories
                .FirstOrDefaultAsync(i => i.BranchId == branchId && i.ProductId == productId);
        }

        public async Task<(IEnumerable<Inventory> Items, int TotalCount)> GetPagedInventoryWithDetailsAsync(int? branchId, string? searchTerm, int? productGroupId, int? supplierId, int page, int pageSize, bool inStockOnly)
        {
            var query = _context.Inventories
                .AsNoTracking()
                .Include(i => i.Product).ThenInclude(p => p.UnitConversions)
                .Include(i => i.Product).ThenInclude(p => p.ProductGroup)
                .Include(i => i.Product).ThenInclude(p => p.Supplier)
                .AsQueryable();

            if (branchId.HasValue)
            {
                query = query.Where(i => i.BranchId == branchId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower().Trim();
                query = query.Where(i =>
                    (i.Product.Name != null && i.Product.Name.ToLower().Contains(term)) ||
                    (i.Product.Sku != null && i.Product.Sku.ToLower().Contains(term)) ||
                    (i.Product.Barcode != null && i.Product.Barcode.ToLower().Contains(term)));
            }

            if (productGroupId.HasValue)
            {
                query = query.Where(i => i.Product.ProductGroupId == productGroupId.Value);
            }

            if (supplierId.HasValue)
            {
                query = query.Where(i => i.Product.SupplierId == supplierId.Value);
            }

            if (inStockOnly)
            {
                query = query.Where(i => i.Quantity > 0);
            }

            int totalCount = await query.CountAsync();

            var items = await query.OrderBy(i => i.Product.Name)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return (items, totalCount);
        }
    }
}