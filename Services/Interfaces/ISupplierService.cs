using BusinessObjects;
using BusinessObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface ISupplierService
    {
        IEnumerable<SupplierResponseDTO> GetAllSuppliers();
        SupplierResponseDTO GetSupplierById(int id);
        void SaveSupplier(SupplierRequestDTO request);
        void UpdateSupplier(int id, SupplierRequestDTO request);

        void DeleteSupplier(int id);
    }
}
