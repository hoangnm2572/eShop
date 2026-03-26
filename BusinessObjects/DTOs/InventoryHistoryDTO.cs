using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObjects.DTOs
{
    public class InventoryLedgerHistoryDTO
    {
        public long Id { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; } = null!;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductSku { get; set; } = null!;
        public string TransactionType { get; set; } = null!;
        public int QuantityChange { get; set; }
        public string? ReferenceCode { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatorName { get; set; } = null!;
        public int? ApprovedBy { get; set; }
        public string? ApproverName { get; set; }
        public string? PartnerBranchName { get; set; }
    }

    public class InventoryTransferHistoryDTO
    {
        public string? TransferCode { get; set; }
        public int Id { get; set; }
        public int FromBranchId { get; set; }
        public string FromBranchName { get; set; } = null!;
        public int ToBranchId { get; set; }
        public string ToBranchName { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public string CreatorName { get; set; } = null!;

        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedBy { get; set; }
        public string? ApproverName { get; set; }

        public List<InventoryTransferDetailHistoryDTO> Items { get; set; } = new List<InventoryTransferDetailHistoryDTO>();
    }

    public class InventoryTransferDetailHistoryDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductSku { get; set; } = null!;
        public int Quantity { get; set; }
    }

    public class InventoryLedgerGroupedDTO
    {
        public string ReferenceCode { get; set; } = null!;
        public string TransactionType { get; set; } = null!;
        public int BranchId { get; set; }
        public string BranchName { get; set; } = null!;

        public string? PartnerBranchName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CreatorName { get; set; } = null!;
        public int? ApprovedBy { get; set; }
        public string? ApproverName { get; set; }
        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
        public List<InventoryLedgerHistoryDTO> Details { get; set; } = new List<InventoryLedgerHistoryDTO>();
    }
}
