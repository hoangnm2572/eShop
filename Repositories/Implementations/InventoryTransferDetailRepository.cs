using BusinessObjects;
using Repositories.Base;
using Repositories.dbContext;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Implementations
{
    public class InventoryTransferDetailRepository : BaseRepository<InventoryTransferDetail>, IInventoryTransferDetailRepository
    {
        public InventoryTransferDetailRepository(eShopDbContext context) : base(context)
        {
        }
    }
}
