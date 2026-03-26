using BusinessObjects;
using Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IInventoryTransferDetailRepository : IBaseRepository<InventoryTransferDetail>
    {
        Task<IEnumerable<InventoryTransferDetail>> GetByTransferIdAsync(int transferId);
    }
}