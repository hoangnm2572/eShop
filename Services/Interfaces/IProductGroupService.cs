using BusinessObjects.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IProductGroupService
    {
        Task<IEnumerable<ProductGroupResponseDTO>> GetAllProductGroupsAsync();
        Task<ProductGroupResponseDTO> GetProductGroupByIdAsync(int id);
        Task SaveProductGroupAsync(ProductGroupRequestDTO request);
        Task UpdateProductGroupAsync(int id, ProductGroupRequestDTO request);
        Task DeleteProductGroupAsync(int id);
    }
}