using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.dbContext;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(eShopDbContext context) : base(context)
        {
        }

        public Product GetByBarcode(string barcode)
        {
            return _dbSet.FirstOrDefault(p => p.Barcode == barcode);
        }

        public IEnumerable<Product> GetAllWithDetails()
        {
            return _dbSet
                .Include(p => p.ProductGroup)
                .Include(p => p.Supplier)
                .Include(p => p.UnitConversions)
                .ToList();
        }
    }
}
