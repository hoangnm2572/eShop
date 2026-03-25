using Repositories.Interfaces;
using Services.Interfaces;
using BusinessObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

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

        public IEnumerable<UserResponseDTO> GetAllUsers()
        {
            var users = _userRepo.GetAll();
            var branches = _branchRepo.GetAll();

            return users.Select(u => new UserResponseDTO
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                BranchId = u.BranchId ?? 0,
                BranchName = branches.FirstOrDefault(b => b.Id == u.BranchId)?.Name ?? "Hệ thống",
                IsActive = u.IsActive ?? false
            });
        }

        public UserResponseDTO GetUserById(int id)
        {
            var u = _userRepo.GetById(id);
            if (u == null) return null;

            return new UserResponseDTO
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role,
                BranchId = u.BranchId ?? 0
            };
        }

        public void DeleteUser(int id)
        {
            var user = _userRepo.GetById(id);
            if (user == null) throw new Exception("Người dùng không tồn tại");

            if (user.Username.ToLower() == "admin")
                throw new Exception("Không được phép vô hiệu hóa tài khoản quản trị hệ thống mặc định");
            user.IsActive = false;

            _userRepo.Update(user);
        }

        public void ActivateUser(int id)
        {
            var user = _userRepo.GetById(id);
            if (user == null) throw new Exception("Người dùng không tồn tại");

            user.IsActive = true;
            _userRepo.Update(user);
        }
    }
}
