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
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(eShopDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetByBarcodeAsync(string barcode)
        {
            return await _context.Set<Product>()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Barcode == barcode);
        }

        public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
        {
            return await _context.Set<Product>()
                .AsNoTracking()
                .Include(p => p.ProductGroup)
                .Include(p => p.Supplier)
                .Include(p => p.UnitConversions)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedProductsWithDetailsAsync(
            int page,
            int pageSize,
            string? search,
            int? productGroupId,
            int? supplierId,
            bool? isActive,
            bool? showOnPos)
        {
            var query = _context.Set<Product>()
                .AsNoTracking()
                .Include(p => p.ProductGroup)
                .Include(p => p.Supplier)
                .Include(p => p.UnitConversions)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p =>
                    p.Name.Contains(search) ||
                    p.Barcode.Contains(search) ||
                    p.Sku.Contains(search));
            }

            if (productGroupId.HasValue)
            {
                query = query.Where(p => p.ProductGroupId == productGroupId);
            }

            if (supplierId.HasValue)
            {
                query = query.Where(p => p.SupplierId == supplierId);
            }

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive);
            }

            if (showOnPos.HasValue)
            {
                query = query.Where(p => p.ShowOnPos == showOnPos);
            }

            int totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}