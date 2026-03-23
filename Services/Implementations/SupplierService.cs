using BusinessObjects;
using BusinessObjects.DTOs;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public IEnumerable<SupplierResponseDTO> GetAllSuppliers()
        {
            var suppliers = _supplierRepository.GetAll();
            return suppliers.Select(MapToResponseDTO).ToList();
        }

        public SupplierResponseDTO GetSupplierById(int id)
        {
            var supplier = _supplierRepository.GetById(id);
            if (supplier == null) throw new Exception("Không tìm thấy Nhà cung cấp!");

            return MapToResponseDTO(supplier);
        }

        public void SaveSupplier(SupplierRequestDTO request)
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

            _supplierRepository.Add(supplier);
        }

        public void UpdateSupplier(int id, SupplierRequestDTO request)
        {
            var supplier = _supplierRepository.GetById(id);
            if (supplier == null) throw new Exception("Không tìm thấy Nhà cung cấp!");

            supplier.Name = request.Name;
            supplier.ContactName = request.ContactName;
            supplier.Phone = request.Phone;
            supplier.Email = request.Email;
            supplier.Address = request.Address;

            _supplierRepository.Update(supplier);
        }

        public void DeleteSupplier(int id)
        {
            var supplier = _supplierRepository.GetById(id);
            if (supplier != null)
            {
                _supplierRepository.Delete(supplier);
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