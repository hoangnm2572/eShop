using BusinessObjects;
using Repositories.Base;
using Repositories.dbContext;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class ProductGroupRepository : BaseRepository<ProductGroup>, IProductGroupRepository
    {
        public ProductGroupRepository(eShopDbContext context) : base(context)
        {
        }
    }
}