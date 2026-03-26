using BusinessObjects.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchResponseDTO>> GetAllBranchesAsync(bool onlyActive = true);
        Task<BranchResponseDTO?> GetBranchByIdAsync(int id);
        Task CreateBranchAsync(BranchCreateDTO dto);
        Task UpdateBranchAsync(int id, BranchUpdateDTO dto);
        Task DeleteBranchAsync(int id);
    }
}