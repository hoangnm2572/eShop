using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObjects.DTOs
{
    public class BranchResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string BranchType { get; set; } = null!;
        public string? Address { get; set; }
        public bool IsActive { get; set; }
    }

    public class BranchCreateDTO
    {
        public string Name { get; set; } = null!;
        public string BranchType { get; set; } = "STORE";
        public string? Address { get; set; }
    }

    public class BranchUpdateDTO
    {
        public string Name { get; set; } = null!;
        public string BranchType { get; set; } = null!;
        public string? Address { get; set; }
        public bool IsActive { get; set; }
    }
}
