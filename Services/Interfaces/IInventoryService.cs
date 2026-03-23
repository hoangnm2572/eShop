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

        void DirectImportToBranch(DirectImportRequestDTO request);
        void DirectExportFromBranch(DirectExportRequestDTO request);

        void TransferInventory(DirectTransferRequestDTO request);

        void RequestGoodsFromHub(int requestingBranchId, RequestGoodsDTO request);
        void ApproveGoodsRequest(int transferId, int approvedByUserId);
        void CancelGoodsRequest(int transferId);
        void UpdateGoodsRequest(int transferId, int branchId, RequestGoodsDTO request);
    }
}
