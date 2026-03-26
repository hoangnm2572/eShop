using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Repositories.Base;
using Repositories.dbContext;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class InventoryTransferDetailRepository : BaseRepository<InventoryTransferDetail>, IInventoryTransferDetailRepository
    {
        public InventoryTransferDetailRepository(eShopDbContext context) : base(context) { }

        public async Task<IEnumerable<InventoryTransferDetail>> GetByTransferIdAsync(int transferId)
        {
            return await _context.InventoryTransferDetails
                .AsNoTracking()
                .Where(d => d.TransferId == transferId)
                .ToListAsync();
        }
    }
}