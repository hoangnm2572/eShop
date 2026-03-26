using BusinessObjects;
using Repositories.Base;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<User?> GetUserByBranchAsync(int branchId);
    }
}