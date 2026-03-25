using BusinessObjects;
using BusinessObjects.DTOs;
using Repositories.Base;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IInventoryLedgerRepository _ledgerRepo;
        private readonly IInventoryTransferRepository _transferRepo;
        private readonly IInventoryTransferDetailRepository _transferDetailRepo;

        public InventoryService(
            IInventoryRepository inventoryRepo,
            IInventoryLedgerRepository ledgerRepo,
            IInventoryTransferRepository transferRepo,
            IInventoryTransferDetailRepository transferDetailRepo)
        {
            _inventoryRepo = inventoryRepo;
            _ledgerRepo = ledgerRepo;
            _transferRepo = transferRepo;
            _transferDetailRepo = transferDetailRepo;
        }

        public IEnumerable<InventoryResponseDTO> GetInventoryByBranch(int branchId)
        {
            var inventories = _inventoryRepo.GetInventoryWithDetails(branchId);

            return inventories.Select(i => new InventoryResponseDTO
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,

                Barcode = i.Product?.Barcode,
                Sku = i.Product?.Sku ?? "",
                Name = i.Product?.Name ?? "",
                BaseUnit = i.Product?.BaseUnit ?? "",
                PurchasePrice = i.Product?.PurchasePrice ?? 0,
                SalePrice = i.Product?.SalePrice ?? 0,
                IsActive = i.Product?.IsActive ?? true,
                ShowOnPos = i.Product?.ShowOnPos ?? true,
                ProductGroupName = i.Product?.ProductGroup?.Name,
                SupplierName = i.Product?.Supplier?.Name,

                UnitConversions = i.Product?.UnitConversions.Select(u => new UnitConversionDTO
                {
                    Id = u.Id,
                    UnitName = u.UnitName,
                    ConversionRate = u.ConversionRate,
                    PurchasePrice = u.PurchasePrice,
                    SalePrice = u.SalePrice
                }).ToList() ?? new List<UnitConversionDTO>()
            }).ToList();
        }

        public IEnumerable<InventoryResponseDTO> GetInventory()
        {
            var inventories = _inventoryRepo.GetInventoryWithDetails();

            return inventories.Select(i => new InventoryResponseDTO
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,

                Barcode = i.Product?.Barcode,
                Sku = i.Product?.Sku ?? "",
                Name = i.Product?.Name ?? "",
                BaseUnit = i.Product?.BaseUnit ?? "",
                PurchasePrice = i.Product?.PurchasePrice ?? 0,
                SalePrice = i.Product?.SalePrice ?? 0,
                IsActive = i.Product?.IsActive ?? true,
                ShowOnPos = i.Product?.ShowOnPos ?? true,
                ProductGroupName = i.Product?.ProductGroup?.Name,
                SupplierName = i.Product?.Supplier?.Name,

                UnitConversions = i.Product?.UnitConversions.Select(u => new UnitConversionDTO
                {
                    Id = u.Id,
                    UnitName = u.UnitName,
                    ConversionRate = u.ConversionRate,
                    PurchasePrice = u.PurchasePrice,
                    SalePrice = u.SalePrice
                }).ToList() ?? new List<UnitConversionDTO>()
            }).ToList();
        }

        public void DirectImportToBranch(DirectImportRequestDTO request)
        {
            string refCode = "IMPORT-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            foreach (var item in request.Items)
            {
                AddOrUpdateStock(request.BranchId, item.ProductId, item.Quantity);

                _ledgerRepo.Add(new InventoryLedger
                {
                    BranchId = request.BranchId,
                    ProductId = item.ProductId,
                    TransactionType = "IN",
                    QuantityChange = item.Quantity,
                    ReferenceCode = refCode,
                    CreatedAt = DateTime.Now
                });
            }
        }

        public void TransferInventory(DirectTransferRequestDTO request)
        {
            string tCode = "TRF-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            var transfer = new InventoryTransfer
            {
                TransferCode = tCode,
                FromBranchId = request.FromBranchId,
                ToBranchId = request.ToBranchId,
                Status = "COMPLETED",
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.Now,
                CompletedAt = DateTime.Now,
                Note = request.Note,
                InventoryTransferDetails = request.Items.Select(x => new InventoryTransferDetail
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList()
            };

            _transferRepo.Add(transfer);

            foreach (var item in request.Items)
            {
                DeductStock(request.FromBranchId, item.ProductId, item.Quantity);
                _ledgerRepo.Add(new InventoryLedger
                {
                    BranchId = request.FromBranchId,
                    ProductId = item.ProductId,
                    TransactionType = "TRANSFER_OUT",
                    QuantityChange = -item.Quantity,
                    ReferenceCode = tCode,
                    CreatedAt = DateTime.Now
                });

                AddOrUpdateStock(request.ToBranchId, item.ProductId, item.Quantity);
                _ledgerRepo.Add(new InventoryLedger
                {
                    BranchId = request.ToBranchId,
                    ProductId = item.ProductId,
                    TransactionType = "TRANSFER_IN",
                    QuantityChange = item.Quantity,
                    ReferenceCode = tCode,
                    CreatedAt = DateTime.Now
                });
            }
        }

        public void DirectExportFromBranch(DirectExportRequestDTO request)
        {
            if (request.BranchId != 1)
                throw new Exception("Chỉ Kho tổng (Chi nhánh 1) mới có quyền xuất kho trực tiếp!");

            string refCode = "EXPORT-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            foreach (var item in request.Items)
            {
                DeductStock(request.BranchId, item.ProductId, item.Quantity);

                _ledgerRepo.Add(new InventoryLedger
                {
                    BranchId = request.BranchId,
                    ProductId = item.ProductId,
                    TransactionType = "OUT",
                    QuantityChange = -item.Quantity,
                    ReferenceCode = refCode,
                    CreatedAt = DateTime.Now
                });
            }
        }

        public void RequestGoodsFromHub(int requestingBranchId, RequestGoodsDTO request)
        {
            if (requestingBranchId == 1)
                throw new Exception("Kho tổng không thể tự xin hàng của chính mình!");

            string tCode = "REQ-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            var transfer = new InventoryTransfer
            {
                TransferCode = tCode,
                FromBranchId = 1,
                ToBranchId = requestingBranchId,
                Status = "REQUESTED",
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.Now,
                Note = request.Note,
                InventoryTransferDetails = request.Items.Select(x => new InventoryTransferDetail
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                }).ToList()
            };

            _transferRepo.Add(transfer);
        }

        public void ApproveGoodsRequest(int transferId, ApproveGoodsRequestDTO request)
        {
            var transfer = _transferRepo.GetTransferWithDetails(transferId);

            if (transfer == null) throw new Exception("Không tìm thấy phiếu yêu cầu!");
            if (transfer.Status != "REQUESTED") throw new Exception("Phiếu này đã được xử lý hoặc không hợp lệ!");

            transfer.Status = "COMPLETED";
            transfer.CompletedAt = DateTime.Now;

            var oldDetails = transfer.InventoryTransferDetails.ToList();
            foreach (var oldItem in oldDetails)
            {
                _transferDetailRepo.Delete(oldItem);
            }

            transfer.InventoryTransferDetails = request.ItemsToApprove.Select(x => new InventoryTransferDetail
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                TransferId = transferId
            }).ToList();

            foreach (var item in request.ItemsToApprove)
            {
                DeductStock(transfer.FromBranchId, item.ProductId, item.Quantity);

                _ledgerRepo.Add(new InventoryLedger
                {
                    BranchId = transfer.FromBranchId,
                    ProductId = item.ProductId,
                    TransactionType = "TRANSFER_OUT",
                    QuantityChange = -item.Quantity,
                    ReferenceCode = transfer.TransferCode,
                    CreatedAt = DateTime.Now
                });

                AddOrUpdateStock(transfer.ToBranchId, item.ProductId, item.Quantity);

                _ledgerRepo.Add(new InventoryLedger
                {
                    BranchId = transfer.ToBranchId,
                    ProductId = item.ProductId,
                    TransactionType = "TRANSFER_IN",
                    QuantityChange = item.Quantity,
                    ReferenceCode = transfer.TransferCode,
                    CreatedAt = DateTime.Now
                });
            }

            _transferRepo.Update(transfer);
        }

        public void CancelGoodsRequest(int transferId)
        {
            var transfer = _transferRepo.GetById(transferId);

            if (transfer == null) throw new Exception("Không tìm thấy phiếu yêu cầu!");
            if (transfer.Status != "REQUESTED") throw new Exception("Chỉ có thể hủy phiếu đang ở trạng thái chờ duyệt (REQUESTED)!");

            transfer.Status = "CANCELLED";
            transfer.CompletedAt = DateTime.Now;

            _transferRepo.Update(transfer);
        }

        public void UpdateGoodsRequest(int transferId, int branchId, RequestGoodsDTO request)
        {
            var transfer = _transferRepo.GetTransferWithDetails(transferId);

            if (transfer == null) throw new Exception("Không tìm thấy phiếu yêu cầu!");
            if (transfer.ToBranchId != branchId) throw new Exception("Bạn không có quyền sửa phiếu của chi nhánh khác!");
            if (transfer.Status != "REQUESTED") throw new Exception("Chỉ có thể sửa phiếu đang ở trạng thái chờ duyệt!");

            if (transfer.CreatedAt.HasValue)
            {
                var timePassed = (DateTime.Now - transfer.CreatedAt.Value).TotalMinutes;
                if (timePassed > 30)
                {
                    throw new Exception($"Đã quá 30 phút kể từ lúc tạo phiếu (thực tế: {Math.Round(timePassed)} phút). Bạn không thể sửa đổi nữa, vui lòng liên hệ Kho tổng hoặc tạo phiếu mới!");
                }
            }

            transfer.Note = request.Note;

            var oldDetails = transfer.InventoryTransferDetails.ToList();
            foreach (var oldItem in oldDetails)
            {
                _transferDetailRepo.Delete(oldItem);
            }

            transfer.InventoryTransferDetails = request.Items.Select(x => new InventoryTransferDetail
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                TransferId = transferId
            }).ToList();

            _transferRepo.Update(transfer);
        }

        private void AddOrUpdateStock(int branchId, int productId, int qty)
        {
            var inv = _inventoryRepo.GetAll().FirstOrDefault(i => i.BranchId == branchId && i.ProductId == productId);
            if (inv != null)
            {
                inv.Quantity += qty;
                _inventoryRepo.Update(inv);
            }
            else
            {
                _inventoryRepo.Add(new Inventory { BranchId = branchId, ProductId = productId, Quantity = qty });
            }
        }

        private void DeductStock(int branchId, int productId, int qty)
        {
            var inv = _inventoryRepo.GetAll().FirstOrDefault(i => i.BranchId == branchId && i.ProductId == productId);
            if (inv == null || inv.Quantity < qty)
                throw new Exception($"Lỗi: Chi nhánh {branchId} không đủ số lượng tồn kho cho sản phẩm ID {productId} để xuất!");

            inv.Quantity -= qty;
            _inventoryRepo.Update(inv);
        }
    }
}