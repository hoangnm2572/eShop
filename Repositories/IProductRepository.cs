using BussinessObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetProducts();
        Product GetProductById(int id);
        void SaveProduct(Product p);
        void UpdateProduct(Product p);
        void DeleteProduct(int id);
    }
}
