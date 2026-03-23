using BusinessObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IBranchService
    {
        IEnumerable<BranchResponseDTO> GetAllBranches(bool onlyActive = true);
        BranchResponseDTO? GetBranchById(int id);
        void CreateBranch(BranchCreateDTO dto);
        void UpdateBranch(int id, BranchUpdateDTO dto);
        void DeleteBranch(int id);
    }
}
