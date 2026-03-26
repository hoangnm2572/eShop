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
    public class ProductGroupService : IProductGroupService
    {
        private readonly IProductGroupRepository _productGroupRepository;

        public ProductGroupService(IProductGroupRepository productGroupRepository)
        {
            _productGroupRepository = productGroupRepository;
        }

        public async Task<IEnumerable<ProductGroupResponseDTO>> GetAllProductGroupsAsync()
        {
            var groups = await _productGroupRepository.GetAllAsync();
            return groups.Select(MapToResponseDTO).ToList();
        }

        public async Task<ProductGroupResponseDTO> GetProductGroupByIdAsync(int id)
        {
            var group = await _productGroupRepository.GetByIdAsync(id);
            if (group == null) throw new Exception("Không tìm thấy Nhóm hàng hóa!");

            return MapToResponseDTO(group);
        }

        public async Task SaveProductGroupAsync(ProductGroupRequestDTO request)
        {
            var group = new ProductGroup
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive
            };

            await _productGroupRepository.AddAsync(group);
        }

        public async Task UpdateProductGroupAsync(int id, ProductGroupRequestDTO request)
        {
            var group = await _productGroupRepository.GetByIdAsync(id);
            if (group == null) throw new Exception("Không tìm thấy Nhóm hàng hóa!");

            group.Name = request.Name;
            group.Description = request.Description;
            group.IsActive = request.IsActive;

            await _productGroupRepository.UpdateAsync(group);
        }

        public async Task DeleteProductGroupAsync(int id)
        {
            var group = await _productGroupRepository.GetByIdAsync(id);
            if (group != null)
            {
                await _productGroupRepository.DeleteAsync(group);
            }
        }

        private ProductGroupResponseDTO MapToResponseDTO(ProductGroup g)
        {
            return new ProductGroupResponseDTO
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                IsActive = g.IsActive
            };
        }
    }
}