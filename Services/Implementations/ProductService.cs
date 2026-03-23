using BusinessObjects;
using BusinessObjects.DTOs;
using Repositories.Interfaces;
using Services.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IEnumerable<ProductResponseDTO> GetProducts()
        {
            var products = _productRepository.GetAllWithDetails();
            return products.Select(MapToResponseDTO).ToList();
        }

        public ProductResponseDTO GetProductById(int id)
        {
            var p = _productRepository.GetAllWithDetails().FirstOrDefault(x => x.Id == id);
            if (p == null) throw new Exception("Không tìm thấy sản phẩm!");
            return MapToResponseDTO(p);
        }

        public ProductResponseDTO GetProductByBarcode(string barcode)
        {
            var p = _productRepository.GetAllWithDetails().FirstOrDefault(x => x.Barcode == barcode);
            if (p == null) throw new Exception("Không tìm thấy sản phẩm!");
            return MapToResponseDTO(p);
        }

        public void SaveProduct(ProductRequestDTO request)
        {
            var product = new Product
            {
                Barcode = request.Barcode,
                Sku = request.Sku,
                Name = request.Name,
                BaseUnit = request.BaseUnit,
                PurchasePrice = request.PurchasePrice,
                SalePrice = request.SalePrice,
                IsActive = request.IsActive,
                ShowOnPos = request.ShowOnPos,
                ProductGroupId = request.ProductGroupId,
                SupplierId = request.SupplierId,
                CreatedAt = DateTime.Now,

                UnitConversions = request.UnitConversions.Select(u => new UnitConversion
                {
                    UnitName = u.UnitName,
                    ConversionRate = u.ConversionRate,
                    PurchasePrice = u.PurchasePrice,
                    SalePrice = u.SalePrice
                }).ToList()
            };

            _productRepository.Add(product);
        }

        public void UpdateProduct(int id, ProductRequestDTO request)
        {
            var product = _productRepository.GetAllWithDetails().FirstOrDefault(x => x.Id == id);
            if (product == null) throw new Exception("Không tìm thấy sản phẩm!");

            product.Barcode = request.Barcode;
            product.Sku = request.Sku;
            product.Name = request.Name;
            product.BaseUnit = request.BaseUnit;
            product.PurchasePrice = request.PurchasePrice;
            product.SalePrice = request.SalePrice;
            product.IsActive = request.IsActive;
            product.ShowOnPos = request.ShowOnPos;
            product.ProductGroupId = request.ProductGroupId;
            product.SupplierId = request.SupplierId;

            product.UnitConversions.Clear();
            foreach (var u in request.UnitConversions)
            {
                product.UnitConversions.Add(new UnitConversion
                {
                    UnitName = u.UnitName,
                    ConversionRate = u.ConversionRate,
                    PurchasePrice = u.PurchasePrice,
                    SalePrice = u.SalePrice
                });
            }

            _productRepository.Update(product);
        }

        public void DeleteProduct(int id)
        {
            var product = _productRepository.GetById(id);
            if (product != null) _productRepository.Delete(product);
        }

        private ProductResponseDTO MapToResponseDTO(Product p)
        {
            return new ProductResponseDTO
            {
                Id = p.Id,
                Barcode = p.Barcode,
                Sku = p.Sku,
                Name = p.Name,
                BaseUnit = p.BaseUnit,
                PurchasePrice = p.PurchasePrice,
                SalePrice = p.SalePrice,
                IsActive = p.IsActive,
                ShowOnPos = p.ShowOnPos,
                CreatedAt = p.CreatedAt,
                ProductGroupId = p.ProductGroupId,
                ProductGroupName = p.ProductGroup?.Name,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier?.Name,
                UnitConversions = p.UnitConversions.Select(u => new UnitConversionDTO
                {
                    Id = u.Id,
                    UnitName = u.UnitName,
                    ConversionRate = u.ConversionRate,
                    PurchasePrice = u.PurchasePrice,
                    SalePrice = u.SalePrice
                }).ToList()
            };
        }
    }
}