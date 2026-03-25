using System;
using System.Collections.Generic;

namespace BusinessObjects.DTOs
{
    public class InventoryResponseDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public string? Barcode { get; set; }
        public string Sku { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string BaseUnit { get; set; } = null!;
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsActive { get; set; }
        public bool ShowOnPos { get; set; }
        public string? ProductGroupName { get; set; }
        public string? SupplierName { get; set; }
        public List<UnitConversionDTO> UnitConversions { get; set; } = new List<UnitConversionDTO>();
    }

    public class InventoryTransactionItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class DirectImportRequestDTO
    {
        public int BranchId { get; set; }
        public int CreatedBy { get; set; }
        public string Note { get; set; } = "Nhập hàng từ Nhà Cung Cấp";
        public List<InventoryTransactionItemDTO> Items { get; set; } = new List<InventoryTransactionItemDTO>();
    }

    public class DirectExportRequestDTO
    {
        public int BranchId { get; set; }
        public int CreatedBy { get; set; }
        public string Note { get; set; } = "Xuất kho nội bộ";
        public List<InventoryTransactionItemDTO> Items { get; set; } = new List<InventoryTransactionItemDTO>();
    }

    public class DirectTransferRequestDTO
    {
        public int FromBranchId { get; set; }
        public int ToBranchId { get; set; }
        public string? Note { get; set; }
        public int CreatedBy { get; set; }
        public List<InventoryTransactionItemDTO> Items { get; set; } = new List<InventoryTransactionItemDTO>();
    }

    public class RequestGoodsDTO
    {
        public string? Note { get; set; } = "Cửa hàng xin cấp thêm hàng";
        public int CreatedBy { get; set; }
        public List<InventoryTransactionItemDTO> Items { get; set; } = new List<InventoryTransactionItemDTO>();
    }

    public class ApproveGoodsRequestDTO
    {
        public int UserId { get; set; }
        public List<InventoryTransactionItemDTO> ItemsToApprove { get; set; } = new List<InventoryTransactionItemDTO>();
    }
}