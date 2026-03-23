using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.dbContext;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Implementations
{
    public class InventoryTransferRepository : BaseRepository<InventoryTransfer>, IInventoryTransferRepository
    {
        private readonly eShopDbContext _context;

        public InventoryTransferRepository(eShopDbContext context) : base(context)
        {
            _context = context;
        }

        public InventoryTransfer? GetTransferWithDetails(int transferId)
        {
            return _context.InventoryTransfers
                .Include(t => t.InventoryTransferDetails)
                .FirstOrDefault(t => t.Id == transferId);
        }
    }
}
