using BusinessObjects;
using Repositories.Base;
using Repositories.dbContext;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class BranchRepository : BaseRepository<Branch>, IBranchRepository
    {
        public BranchRepository(eShopDbContext context) : base(context) { }
    }
}