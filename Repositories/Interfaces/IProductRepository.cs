using BusinessObjects;
using Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<Product?> GetByBarcodeAsync(string barcode);
        Task<IEnumerable<Product>> GetAllWithDetailsAsync();
        Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedProductsWithDetailsAsync(
            int page,
            int pageSize,
            string? search,
            int? productGroupId,
            int? supplierId,
            bool? isActive,
            bool? showOnPos
        );
    }
}