using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObjects.DTOs
{
    public class RegisterRequestDTO
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public int? BranchId { get; set; }
        public string Role { get; set; } = "STORE";
    }

    public class LoginRequestDTO
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class AuthResponseDTO
    {
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public int BranchId { get; set; }
        public string Role { get; set; } = null!;
    }

    public class ChangePasswordRequestDTO
    {
        public int TargetUserId { get; set; }
        public string NewPassword { get; set; } = null!;
    }
}