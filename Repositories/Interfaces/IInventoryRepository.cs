using BusinessObjects;
using Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Interfaces
{
    public interface IInventoryRepository : IBaseRepository<Inventory>
    {
        IEnumerable<Inventory> GetInventoryWithDetails(int branchId);
        IEnumerable<Inventory> GetInventoryWithDetails();
    }
}
