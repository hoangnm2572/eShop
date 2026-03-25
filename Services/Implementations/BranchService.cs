using Azure.Core;
using BusinessObjects;
using BusinessObjects.DTOs;
using Repositories.Interfaces;
using Services.Helpers;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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

        public IEnumerable<BranchResponseDTO> GetAllBranches(bool onlyActive = true)
        {
            var query = _branchRepo.GetAll();

            if (onlyActive)
            {
                query = query.Where(b => b.IsActive == true);
            }

            return query.Select(b => new BranchResponseDTO
            {
                Id = b.Id,
                Name = b.Name,
                BranchType = b.BranchType,
                Address = b.Address,
                IsActive = b.IsActive ?? false
            }).ToList();
        }

        public BranchResponseDTO? GetBranchById(int id)
        {
            var b = _branchRepo.GetById(id);
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

        public void CreateBranch(BranchCreateDTO dto)
        {
            var branch = new Branch
            {
                Name = dto.Name,
                BranchType = dto.BranchType.ToUpper(),
                Address = dto.Address,
                IsActive = true
            };

            _branchRepo.Add(branch);

            string username = StringHelper.GenerateUsername(dto.Name);

            var existingUser = _userRepo.GetAll().FirstOrDefault(u => u.Username == username);
            if (existingUser != null)
            {

                username = $"{username}_{branch.Id}";
            }

            string defaultPassword = "123456";
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword);

            var newUser = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                BranchId = branch.Id,
                Role = "STORE"
            };

            _userRepo.Add(newUser);
        }

        public void UpdateBranch(int id, BranchUpdateDTO dto)
        {
            var branch = _branchRepo.GetById(id);
            if (branch == null) throw new Exception("Không tìm thấy chi nhánh!");

            branch.Name = dto.Name;
            branch.BranchType = dto.BranchType.ToUpper();
            branch.Address = dto.Address;
            branch.IsActive = dto.IsActive;

            _branchRepo.Update(branch);
        }

        public void DeleteBranch(int id)
        {
            if (id == 1) throw new Exception("Không được phép đóng cửa hoặc xóa Kho tổng (Hub)!");

            var branch = _branchRepo.GetById(id);
            if (branch == null) throw new Exception("Không tìm thấy chi nhánh!");

            branch.IsActive = false;

            _branchRepo.Update(branch);
        }
    }
}
