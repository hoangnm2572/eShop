using Repositories.Interfaces;
using Services.Interfaces;
using BusinessObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IBranchRepository _branchRepo;

        public UserService(IUserRepository userRepo, IBranchRepository branchRepo)
        {
            _userRepo = userRepo;
            _branchRepo = branchRepo;
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllAsync();
            var branches = await _branchRepo.GetAllAsync();

            return users.Select(u => new UserResponseDTO
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Role = u.Role,
                BranchId = u.BranchId ?? 0,
                BranchName = branches.FirstOrDefault(b => b.Id == u.BranchId)?.Name ?? "Hệ thống",
                IsActive = u.IsActive ?? false
            }).ToList();
        }

        public async Task<UserResponseDTO?> GetUserByIdAsync(int id)
        {
            var u = await _userRepo.GetByIdAsync(id);
            if (u == null) return null;

            return new UserResponseDTO
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Role = u.Role,
                BranchId = u.BranchId ?? 0
            };
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) throw new Exception("Người dùng không tồn tại");

            if (user.Username.ToLower() == "admin")
                throw new Exception("Không được phép vô hiệu hóa tài khoản quản trị hệ thống mặc định");

            user.IsActive = false;
            await _userRepo.UpdateAsync(user);
        }

        public async Task ActivateUserAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) throw new Exception("Người dùng không tồn tại");

            user.IsActive = true;
            await _userRepo.UpdateAsync(user);
        }

        public async Task UpdateUserAsync(int userId, UpdateUserRequestDTO request)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) throw new Exception("Không tìm thấy tài khoản!");

            if (user.Username.ToLower() == "admin" && request.Role != "ADMIN")
                throw new Exception("Không thể hạ quyền của Admin gốc!");

            user.FullName = request.FullName;
            user.Role = request.Role;

            await _userRepo.UpdateAsync(user);
        }
    }
}