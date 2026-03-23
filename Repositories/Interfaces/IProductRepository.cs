using BusinessObjects;
using Repositories.Base;

namespace Repositories.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Product GetByBarcode(string barcode);
        IEnumerable<Product> GetAllWithDetails();
    }
}
