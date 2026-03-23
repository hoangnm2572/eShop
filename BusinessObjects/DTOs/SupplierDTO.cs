using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObjects.DTOs
{
    public class SupplierResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SupplierRequestDTO
    {
        public string Name { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
    }
}
