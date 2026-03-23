using System;
using System.Collections.Generic;

namespace BusinessObjects.DTOs
{
    public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string? Barcode { get; set; }
        public string Sku { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string BaseUnit { get; set; } = null!;
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsActive { get; set; }
        public bool ShowOnPos { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int? ProductGroupId { get; set; }
        public string? ProductGroupName { get; set; }

        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }

        public List<UnitConversionDTO> UnitConversions { get; set; } = new List<UnitConversionDTO>();
    }

    public class UnitConversionDTO
    {
        public int Id { get; set; }
        public string UnitName { get; set; } = null!;
        public int ConversionRate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
    }
    public class ProductRequestDTO
    {
        public string? Barcode { get; set; }
        public string Sku { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string BaseUnit { get; set; } = null!;
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsActive { get; set; } = true;
        public bool ShowOnPos { get; set; } = true;

        public int? ProductGroupId { get; set; }
        public int? SupplierId { get; set; }

        public List<UnitConversionRequestDTO> UnitConversions { get; set; } = new List<UnitConversionRequestDTO>();
    }

    public class UnitConversionRequestDTO
    {
        public string UnitName { get; set; } = null!;
        public int ConversionRate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
    }
}