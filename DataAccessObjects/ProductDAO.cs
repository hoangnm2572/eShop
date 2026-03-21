using BussinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessObjects
{
    public class ProductDAO
    {
        private static ProductDAO instance = null;
        private static readonly object instanceLock = new object();

        private ProductDAO() { }

        public static ProductDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ProductDAO();
                    }
                    return instance;
                }
            }
        }

        public IEnumerable<Product> GetProducts()
        {
            var products = new List<Product>();
            try
            {
                using var context = new eShopDbContext();
                products = context.Products.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return products;
        }

        public Product GetProductById(int id)
        {
            Product product = null;
            try
            {
                using var context = new eShopDbContext();
                product = context.Products.SingleOrDefault(p => p.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return product;
        }

        public void SaveProduct(Product p)
        {
            try
            {
                Product product = GetProductById(p.Id);
                if (product == null)
                {
                    using var context = new eShopDbContext();
                    context.Products.Add(p);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The product already exists.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void UpdateProduct(Product p)
        {
            try
            {
                Product product = GetProductById(p.Id);
                if (product != null)
                {
                    using var context = new eShopDbContext();
                    context.Products.Update(p);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The product does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void DeleteProduct(int id)
        {
            try
            {
                Product product = GetProductById(id);
                if (product != null)
                {
                    using var context = new eShopDbContext();
                    context.Products.Remove(product);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The product does not exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
