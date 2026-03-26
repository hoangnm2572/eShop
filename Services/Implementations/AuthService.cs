using BusinessObjects;
using BusinessObjects.DTOs;
using Microsoft.IdentityModel.Tokens;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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

        public async Task RegisterAsync(RegisterRequestDTO request)
        {
            if (request.Role == "STORE")
            {
                if (!request.BranchId.HasValue)
                    throw new Exception("Vui lòng chọn chi nhánh!");

                var branch = await _branchRepo.GetByIdAsync(request.BranchId.Value);
                if (branch == null) throw new Exception("Chi nhánh không tồn tại!");

                var existingUserInBranch = await _userRepo.GetUserByBranchAsync(request.BranchId.Value);
                if (existingUserInBranch != null)
                    throw new Exception($"Chi nhánh '{branch.Name}' đã có tài khoản rồi!");
            }

            bool usernameExists = await _userRepo.IsUsernameExistsAsync(request.Username);
            if (usernameExists) throw new Exception("Tên đăng nhập đã tồn tại!");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                FullName = request.FullName,
                BranchId = request.Role == "STORE" ? request.BranchId : null,
                Role = request.Role,
                IsActive = true
            };

            await _userRepo.AddAsync(newUser);
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request, string jwtKey, string jwtIssuer)
        {
            var user = await _userRepo.GetByUsernameAsync(request.Username);

            if (user == null || user.IsActive == false)
                throw new Exception("Tên đăng nhập hoặc mật khẩu không chính xác!");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid) throw new Exception("Tên đăng nhập hoặc mật khẩu không chính xác!");

            if (user.BranchId.HasValue)
            {
                var branch = await _branchRepo.GetByIdAsync(user.BranchId.Value);
                if (branch != null && branch.IsActive == false)
                {
                    throw new Exception("Chi nhánh của bạn hiện đang ngừng hoạt động!");
                }
            }

            return GenerateJwtToken(user, jwtKey, jwtIssuer);
        }

        public async Task ChangePasswordAsync(ChangePasswordRequestDTO request)
        {
            var user = await _userRepo.GetByIdAsync(request.TargetUserId);
            if (user == null) throw new Exception("Không tìm thấy tài khoản!");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepo.UpdateAsync(user);
        }

        public async Task ChangePasswordByBranchAsync(int branchId, string newPassword)
        {
            var user = await _userRepo.GetUserByBranchAsync(branchId);
            if (user == null) throw new Exception("Không tìm thấy tài khoản gắn với chi nhánh này!");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepo.UpdateAsync(user);
        }

        private AuthResponseDTO GenerateJwtToken(User user, string jwtKey, string jwtIssuer)
        {
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
            return new AuthResponseDTO
            {
                Token = tokenHandler.WriteToken(token),
                UserId = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                BranchId = user.BranchId ?? 0,
                Role = user.Role ?? "STORE"
            };
        }
    }
}