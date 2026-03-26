using BusinessObjects;
using Repositories.Base;
using Repositories.dbContext;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(eShopDbContext context) : base(context)
        {
        }
    }
}