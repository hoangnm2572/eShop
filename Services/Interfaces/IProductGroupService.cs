using BusinessObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IProductGroupService
    {
        IEnumerable<ProductGroupResponseDTO> GetAllProductGroups();
        ProductGroupResponseDTO GetProductGroupById(int id);

        void SaveProductGroup(ProductGroupRequestDTO request);
        void UpdateProductGroup(int id, ProductGroupRequestDTO request);
        void DeleteProductGroup(int id);
    }
}
