using BusinessObjects.DTOs;
using System;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IInventoryService
    {
        Task<PagedResponseDTO<InventoryResponseDTO>> GetInventoryByBranchAsync(int branchId, string? searchTerm = null, int? productGroupId = null, int? supplierId = null, int page = 1, int pageSize = 10, bool inStockOnly = false);
        Task<PagedResponseDTO<InventoryResponseDTO>> GetInventoryAsync(string? searchTerm = null, int? productGroupId = null, int? supplierId = null, int page = 1, int pageSize = 10, bool inStockOnly = false);

        Task DirectImportToBranchAsync(DirectImportRequestDTO request);
        Task DirectExportFromBranchAsync(DirectExportRequestDTO request);
        Task TransferInventoryAsync(DirectTransferRequestDTO request);
        Task RequestGoodsFromHubAsync(int requestingBranchId, RequestGoodsDTO request);
        Task ApproveGoodsRequestAsync(int transferId, ApproveGoodsRequestDTO request, int userId);
        Task CancelGoodsRequestAsync(int transferId, int userId);
        Task CompleteGoodsRequestAsync(int transferId, int userId);
        Task UpdateGoodsRequestAsync(int transferId, int branchId, RequestGoodsDTO request);

        Task<PagedResponseDTO<InventoryLedgerHistoryDTO>> GetInventoryLedgerHistoryAsync(
            int? branchId = null, string? searchTerm = null, string? transactionType = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10);

        Task<PagedResponseDTO<InventoryTransferHistoryDTO>> GetInventoryTransferHistoryAsync(
            int? branchId = null, string? searchTerm = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10, bool isRequestOnly = false);

        Task<PagedResponseDTO<InventoryLedgerGroupedDTO>> GetInventoryLedgerGroupedHistoryAsync(
            int? branchId = null, string? searchTerm = null, string? transactionType = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10);
    }
}