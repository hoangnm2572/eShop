using BusinessObjects;
using BusinessObjects.DTOs;

namespace Services.Interfaces
{
    public interface IProductService
    {
        IEnumerable<ProductResponseDTO> GetProducts();
        ProductResponseDTO GetProductById(int id);
        ProductResponseDTO GetProductByBarcode(string barcode);
        void SaveProduct(ProductRequestDTO request);
        void UpdateProduct(int id, ProductRequestDTO request);
        void DeleteProduct(int id);
    }
}
