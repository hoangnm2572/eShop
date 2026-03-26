using BusinessObjects;
using BusinessObjects.DTOs;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResponseDTO<ProductResponseDTO>> GetProductsAsync(
            int page,
            int pageSize,
            string? search,
            int? productGroupId,
            int? supplierId,
            bool? isActive,
            bool? showOnPos)
        {
            var (products, totalCount) = await _productRepository.GetPagedProductsWithDetailsAsync(
                page, pageSize, search, productGroupId, supplierId, isActive, showOnPos
            );

            var items = products.Select(MapToResponseDTO).ToList();

            return new PagedResponseDTO<ProductResponseDTO>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ProductResponseDTO> GetProductByIdAsync(int id)
        {
            var products = await _productRepository.GetAllWithDetailsAsync();
            var p = products.FirstOrDefault(x => x.Id == id);

            if (p == null) throw new Exception("Không tìm thấy sản phẩm!");

            return MapToResponseDTO(p);
        }

        public async Task<ProductResponseDTO> GetProductByBarcodeAsync(string barcode)
        {
            var p = await _productRepository.GetByBarcodeAsync(barcode);

            if (p == null) throw new Exception("Không tìm thấy sản phẩm!");

            return MapToResponseDTO(p);
        }

        public async Task SaveProductAsync(ProductRequestDTO request)
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
                CreatedAt = DateTime.Now,
                ProductGroupId = request.ProductGroupId,
                SupplierId = request.SupplierId,
                UnitConversions = request.UnitConversions?.Select(u => new UnitConversion
                {
                    UnitName = u.UnitName,
                    ConversionRate = u.ConversionRate,
                    PurchasePrice = u.PurchasePrice,
                    SalePrice = u.SalePrice
                }).ToList() ?? new List<UnitConversion>()
            };

            await _productRepository.AddAsync(product);
        }

        public async Task UpdateProductAsync(int id, ProductRequestDTO request)
        {
            var products = await _productRepository.GetAllWithDetailsAsync();
            var product = products.FirstOrDefault(p => p.Id == id);

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

            if (request.UnitConversions != null)
            {
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
            }

            await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                await _productRepository.DeleteAsync(product);
            }
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
                UnitConversions = p.UnitConversions?.Select(u => new UnitConversionDTO
                {
                    Id = u.Id,
                    UnitName = u.UnitName,
                    ConversionRate = u.ConversionRate,
                    PurchasePrice = u.PurchasePrice,
                    SalePrice = u.SalePrice
                }).ToList() ?? new List<UnitConversionDTO>()
            };
        }
    }
}