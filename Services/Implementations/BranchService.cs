using BusinessObjects;
using BusinessObjects.DTOs;
using Repositories.Interfaces;
using Services.Helpers;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepo;
        private readonly IUserRepository _userRepo;

        public BranchService(IBranchRepository branchRepo, IUserRepository userRepo)
        {
            _branchRepo = branchRepo;
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<BranchResponseDTO>> GetAllBranchesAsync(bool onlyActive = true)
        {
            var branches = await _branchRepo.GetAllAsync();

            if (onlyActive)
            {
                branches = branches.Where(b => b.IsActive == true);
            }

            return branches.Select(b => new BranchResponseDTO
            {
                Id = b.Id,
                Name = b.Name,
                BranchType = b.BranchType,
                Address = b.Address,
                IsActive = b.IsActive ?? false
            }).ToList();
        }

        public async Task<BranchResponseDTO?> GetBranchByIdAsync(int id)
        {
            var b = await _branchRepo.GetByIdAsync(id);
            if (b == null) return null;

            return new BranchResponseDTO
            {
                Id = b.Id,
                Name = b.Name,
                BranchType = b.BranchType,
                Address = b.Address,
                IsActive = b.IsActive ?? false
            };
        }

        public async Task CreateBranchAsync(BranchCreateDTO dto)
        {
            var branch = new Branch
            {
                Name = dto.Name,
                BranchType = dto.BranchType.ToUpper(),
                Address = dto.Address,
                IsActive = true
            };

            await _branchRepo.AddAsync(branch);

            string username = StringHelper.GenerateUsername(dto.Name);

            bool usernameExists = await _userRepo.IsUsernameExistsAsync(username);
            if (usernameExists)
            {
                username = $"{username}_{branch.Id}";
            }

            string defaultPassword = "123456";
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword);

            var newUser = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                FullName = branch.Name,
                BranchId = branch.Id,
                Role = "STORE",
                IsActive = true
            };

            await _userRepo.AddAsync(newUser);
        }

        public async Task UpdateBranchAsync(int id, BranchUpdateDTO dto)
        {
            var branch = await _branchRepo.GetByIdAsync(id);
            if (branch == null) throw new Exception("Không tìm thấy chi nhánh!");

            branch.Name = dto.Name;
            branch.BranchType = dto.BranchType.ToUpper();
            branch.Address = dto.Address;
            branch.IsActive = dto.IsActive;

            await _branchRepo.UpdateAsync(branch);
        }

        public async Task DeleteBranchAsync(int id)
        {
            if (id == 1) throw new Exception("Không được phép đóng cửa hoặc xóa Kho tổng (Hub)!");

            var branch = await _branchRepo.GetByIdAsync(id);
            if (branch == null) throw new Exception("Không tìm thấy chi nhánh!");

            branch.IsActive = false;

            await _branchRepo.UpdateAsync(branch);
        }
    }
}