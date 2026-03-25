using BusinessObjects;
using BusinessObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IInventoryService
    {
        IEnumerable<InventoryResponseDTO> GetInventoryByBranch(int branchId);
        IEnumerable<InventoryResponseDTO> GetInventory();

        void DirectImportToBranch(DirectImportRequestDTO request);
        void DirectExportFromBranch(DirectExportRequestDTO request);

        void TransferInventory(DirectTransferRequestDTO request);

        void RequestGoodsFromHub(int requestingBranchId, RequestGoodsDTO request);
        void ApproveGoodsRequest(int transferId, ApproveGoodsRequestDTO request, int userId);
        void CancelGoodsRequest(int transferId, int userId);
        void CompleteGoodsRequest(int transferId, int userId);
        void UpdateGoodsRequest(int transferId, int branchId, RequestGoodsDTO request);

        IEnumerable<InventoryLedgerHistoryDTO> GetInventoryLedgerHistory(int? branchId = null);
        IEnumerable<InventoryTransferHistoryDTO> GetInventoryTransferHistory(int? branchId = null);

        IEnumerable<InventoryLedgerGroupedDTO> GetInventoryLedgerGroupedHistory(int? branchId = null);
    }
}
