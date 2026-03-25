using BusinessObjects;
using Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Interfaces
{
    public interface IInventoryLedgerRepository : IBaseRepository<InventoryLedger>
    {
        IEnumerable<InventoryLedger> GetLedgersWithDetails(int? branchId = null);
    }
}
