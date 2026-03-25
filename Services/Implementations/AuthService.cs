using BusinessObjects;
using BusinessObjects.DTOs;
using Microsoft.IdentityModel.Tokens;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IBranchRepository _branchRepo;

        public AuthService(IUserRepository userRepo, IBranchRepository branchRepo)
        {
            _userRepo = userRepo;
            _branchRepo = branchRepo;
        }

        public void Register(RegisterRequestDTO request)
        {
            var branch = _branchRepo.GetById(request.BranchId);
            if (branch == null) throw new Exception("Chi nhánh không tồn tại!");

            if (request.Role == "STORE")
            {
                var existingUserInBranch = _userRepo.GetAll().FirstOrDefault(u => u.BranchId == request.BranchId);
                if (existingUserInBranch != null)
                    throw new Exception($"Chi nhánh '{branch.Name}' đã có tài khoản rồi!");
            }

            var usernameExists = _userRepo.GetAll().Any(u => u.Username == request.Username);
            if (usernameExists) throw new Exception("Tên đăng nhập đã tồn tại!");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                BranchId = request.BranchId,
                Role = request.Role,
                IsActive = true
            };

            _userRepo.Add(newUser);
        }

        public AuthResponseDTO Login(LoginRequestDTO request, string jwtKey, string jwtIssuer)
        {
            var user = _userRepo.GetAll().FirstOrDefault(u => u.Username == request.Username);
            if (user == null || user.IsActive == false)
                throw new Exception("Sai tên đăng nhập hoặc tài khoản đã bị khóa!");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid) throw new Exception("Sai mật khẩu!");

            if (user.BranchId.HasValue)
            {
                var branch = _branchRepo.GetById(user.BranchId.Value);
                if (branch != null && branch.IsActive == false)
                {
                    throw new Exception("Chi nhánh của bạn hiện đang ngừng hoạt động. Không thể truy cập hệ thống!");
                }
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim("FullName", user.FullName ?? ""),
                    new Claim(ClaimTypes.Role, user.Role ?? "STORE"),
                    new Claim("BranchId", user.BranchId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = jwtIssuer,
                Audience = jwtIssuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtString = tokenHandler.WriteToken(token);

            return new AuthResponseDTO
            {
                Token = jwtString,
                UserId = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                BranchId = user.BranchId ?? 0,
                Role = user.Role ?? "STORE"
            };
        }

        public void ChangePassword(ChangePasswordRequestDTO request)
        {
            var user = _userRepo.GetById(request.TargetUserId);
            if (user == null) throw new Exception("Không tìm thấy tài khoản người dùng này!");

            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            user.PasswordHash = newPasswordHash;
            _userRepo.Update(user);
        }

        public void ChangePasswordByBranch(int branchId, string newPassword)
        {
            var user = _userRepo.GetAll().FirstOrDefault(u => u.BranchId == branchId);
            if (user == null) throw new Exception("Không tìm thấy tài khoản gắn với chi nhánh này!");

            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordHash = newPasswordHash;
            _userRepo.Update(user);
        }
    }
}