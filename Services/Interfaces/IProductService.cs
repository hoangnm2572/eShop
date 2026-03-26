using BusinessObjects.DTOs;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IProductService
    {
        Task<PagedResponseDTO<ProductResponseDTO>> GetProductsAsync(
            int page,
            int pageSize,
            string? search,
            int? productGroupId,
            int? supplierId,
            bool? isActive,
            bool? showOnPos
        );

        Task<ProductResponseDTO> GetProductByIdAsync(int id);
        Task<ProductResponseDTO> GetProductByBarcodeAsync(string barcode);
        Task SaveProductAsync(ProductRequestDTO request);
        Task UpdateProductAsync(int id, ProductRequestDTO request);
        Task DeleteProductAsync(int id);
    }
}