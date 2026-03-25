using BusinessObjects;
using Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Interfaces
{
    public interface IInventoryTransferRepository : IBaseRepository<InventoryTransfer>
    {
        InventoryTransfer? GetTransferWithDetails(int transferId);
        IEnumerable<InventoryTransfer> GetAllTransfersWithDetails(int? branchId = null);
    }
}
