using BussinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository iProductRepository;

        public ProductService()
        {
            iProductRepository = new ProductRepository();
        }

        public IEnumerable<Product> GetProducts()
        {
            return iProductRepository.GetProducts();
        }

        public Product GetProductById(int id)
        {
            return iProductRepository.GetProductById(id);
        }

        public void SaveProduct(Product p)
        {
            iProductRepository.SaveProduct(p);
        }

        public void UpdateProduct(Product p)
        {
            iProductRepository.UpdateProduct(p);
        }

        public void DeleteProduct(int id)
        {
            iProductRepository.DeleteProduct(id);
        }
    }
}
