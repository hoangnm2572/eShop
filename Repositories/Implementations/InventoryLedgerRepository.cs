using BusinessObjects;
using Repositories.Base;
using Repositories.dbContext;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Implementations
{
    public class InventoryLedgerRepository : BaseRepository<InventoryLedger>, IInventoryLedgerRepository
    {
        public InventoryLedgerRepository(eShopDbContext context) : base(context)
        {
        }
    }
}
