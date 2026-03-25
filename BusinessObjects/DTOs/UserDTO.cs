using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObjects.DTOs
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateUserRequestDTO
    {
        public string FullName { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}