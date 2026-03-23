using BusinessObjects;
using BusinessObjects.DTOs;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Implementations
{
    public class ProductGroupService : IProductGroupService
    {
        private readonly IProductGroupRepository _productGroupRepository;

        public ProductGroupService(IProductGroupRepository productGroupRepository)
        {
            _productGroupRepository = productGroupRepository;
        }

        public IEnumerable<ProductGroupResponseDTO> GetAllProductGroups()
        {
            var groups = _productGroupRepository.GetAll();
            return groups.Select(MapToResponseDTO).ToList();
        }

        public ProductGroupResponseDTO GetProductGroupById(int id)
        {
            var group = _productGroupRepository.GetById(id);
            if (group == null) throw new Exception("Không tìm thấy Nhóm hàng hóa!");

            return MapToResponseDTO(group);
        }

        public void SaveProductGroup(ProductGroupRequestDTO request)
        {
            var group = new ProductGroup
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = request.IsActive
            };

            _productGroupRepository.Add(group);
        }

        public void UpdateProductGroup(int id, ProductGroupRequestDTO request)
        {
            var group = _productGroupRepository.GetById(id);
            if (group == null) throw new Exception("Không tìm thấy Nhóm hàng hóa!");

            group.Name = request.Name;
            group.Description = request.Description;
            group.IsActive = request.IsActive;

            _productGroupRepository.Update(group);
        }

        public void DeleteProductGroup(int id)
        {
            var group = _productGroupRepository.GetById(id);
            if (group != null)
            {
                _productGroupRepository.Delete(group);
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
