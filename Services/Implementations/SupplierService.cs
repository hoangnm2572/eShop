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
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IEnumerable<SupplierResponseDTO>> GetAllSuppliersAsync()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return suppliers.Select(MapToResponseDTO).ToList();
        }

        public async Task<SupplierResponseDTO> GetSupplierByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null) throw new Exception("Không tìm thấy Nhà cung cấp!");

            return MapToResponseDTO(supplier);
        }

        public async Task SaveSupplierAsync(SupplierRequestDTO request)
        {
            var supplier = new Supplier
            {
                Name = request.Name,
                ContactName = request.ContactName,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address,
                CreatedAt = DateTime.Now
            };

            await _supplierRepository.AddAsync(supplier);
        }

        public async Task UpdateSupplierAsync(int id, SupplierRequestDTO request)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null) throw new Exception("Không tìm thấy Nhà cung cấp!");

            supplier.Name = request.Name;
            supplier.ContactName = request.ContactName;
            supplier.Phone = request.Phone;
            supplier.Email = request.Email;
            supplier.Address = request.Address;

            await _supplierRepository.UpdateAsync(supplier);
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier != null)
            {
                await _supplierRepository.DeleteAsync(supplier);
            }
        }

        private SupplierResponseDTO MapToResponseDTO(Supplier s)
        {
            return new SupplierResponseDTO
            {
                Id = s.Id,
                Name = s.Name,
                ContactName = s.ContactName,
                Phone = s.Phone,
                Email = s.Email,
                Address = s.Address,
                CreatedAt = s.CreatedAt
            };
        }
    }
}