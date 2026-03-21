using BussinessObjects;
using DataAccessObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class ProductRepository : IProductRepository
    {
        public IEnumerable<Product> GetProducts() => ProductDAO.Instance.GetProducts();

        public Product GetProductById(int id) => ProductDAO.Instance.GetProductById(id);

        public void SaveProduct(Product p) => ProductDAO.Instance.SaveProduct(p);

        public void UpdateProduct(Product p) => ProductDAO.Instance.UpdateProduct(p);

        public void DeleteProduct(int id) => ProductDAO.Instance.DeleteProduct(id);
    }
}
