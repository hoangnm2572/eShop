using BusinessObjects.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierResponseDTO>> GetAllSuppliersAsync();
        Task<SupplierResponseDTO> GetSupplierByIdAsync(int id);
        Task SaveSupplierAsync(SupplierRequestDTO request);
        Task UpdateSupplierAsync(int id, SupplierRequestDTO request);
        Task DeleteSupplierAsync(int id);
    }
}