using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.dbContext;
using Repositories.Interfaces;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(eShopDbContext context) : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _context.Set<User>()
                .AnyAsync(u => u.Username == username);
        }

        public async Task<User?> GetUserByBranchAsync(int branchId)
        {
            return await _context.Set<User>()
                .FirstOrDefaultAsync(u => u.BranchId == branchId);
        }
    }
}