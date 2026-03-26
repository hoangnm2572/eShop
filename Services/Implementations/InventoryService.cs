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
                    CreatedAt = DateTime.Now,
                    CreatedBy = request.CreatedBy
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
                ApprovedAt = DateTime.Now,
                ApprovedBy = request.CreatedBy,
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
                    CreatedAt = DateTime.Now,
                    CreatedBy = request.CreatedBy
                });

                AddOrUpdateStock(request.ToBranchId, item.ProductId, item.Quantity);
                _ledgerRepo.Add(new InventoryLedger
                {
                    BranchId = request.ToBranchId,
                    ProductId = item.ProductId,
                    TransactionType = "TRANSFER_IN",
                    QuantityChange = item.Quantity,
                    ReferenceCode = tCode,
                    CreatedAt = DateTime.Now,
                    CreatedBy = request.CreatedBy
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
                    CreatedAt = DateTime.Now,
                    CreatedBy = request.CreatedBy
                });
            }
        }

        public void RequestGoodsFromHub(int requestingBranchId, RequestGoodsDTO request)
        {
            if (requestingBranchId == 1)
                throw new Exception("Kho tổng không thể tự xin hàng của chính mình!");

            string tCode = "REQ-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();

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

        public void ApproveGoodsRequest(int transferId, ApproveGoodsRequestDTO request, int userId)
        {
            var transfer = _transferRepo.GetTransferWithDetails(transferId);

            if (transfer == null) throw new Exception("Không tìm thấy phiếu yêu cầu!");
            if (transfer.Status != "REQUESTED") throw new Exception("Phiếu này đã được xử lý hoặc không hợp lệ!");

            transfer.Status = "PACKED";
            transfer.ApprovedAt = DateTime.Now;
            transfer.ApprovedBy = userId;

            var oldDetails = transfer.InventoryTransferDetails.ToList();
            foreach (var oldItem in oldDetails)
            {
                _transferDetailRepo.Delete(oldItem);
            }

            foreach (var item in request.ItemsToApprove)
            {
                _transferDetailRepo.Add(new InventoryTransferDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TransferId = transferId
                });
            }

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
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId
                });
            }

            _transferRepo.Update(transfer);
        }

        public void CompleteGoodsRequest(int transferId, int userId)
        {
            var transfer = _transferRepo.GetTransferWithDetails(transferId);

            if (transfer == null) throw new Exception("Không tìm thấy phiếu yêu cầu!");
            if (transfer.Status != "PACKED") throw new Exception("Phiếu này chưa được đóng hàng/xuất kho!");

            transfer.Status = "COMPLETED";
            transfer.CompletedAt = DateTime.Now;

            foreach (var item in transfer.InventoryTransferDetails)
            {
                AddOrUpdateStock(transfer.ToBranchId, item.ProductId, item.Quantity);

                _ledgerRepo.Add(new InventoryLedger
                {
                    BranchId = transfer.ToBranchId,
                    ProductId = item.ProductId,
                    TransactionType = "TRANSFER_IN",
                    QuantityChange = item.Quantity,
                    ReferenceCode = transfer.TransferCode,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId
                });
            }

            _transferRepo.Update(transfer);
        }

        public void CancelGoodsRequest(int transferId, int userId)
        {
            var transfer = _transferRepo.GetById(transferId);

            if (transfer == null) throw new Exception("Không tìm thấy phiếu yêu cầu!");
            if (transfer.Status != "REQUESTED") throw new Exception("Chỉ có thể hủy phiếu đang ở trạng thái chờ duyệt (REQUESTED)!");

            transfer.Status = "CANCELLED";
            transfer.CompletedAt = DateTime.Now;
            transfer.ApprovedAt = DateTime.Now;
            transfer.ApprovedBy = userId;

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

            foreach (var item in request.Items)
            {
                _transferDetailRepo.Add(new InventoryTransferDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TransferId = transferId
                });
            }

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

        public IEnumerable<InventoryLedgerHistoryDTO> GetInventoryLedgerHistory(int? branchId = null)
        {
            var ledgers = _ledgerRepo.GetLedgersWithDetails(branchId);
            var transfers = _transferRepo.GetAllTransfersWithDetails(null).ToList();

            return ledgers.Select(l =>
            {
                var transfer = transfers.FirstOrDefault(t => t.TransferCode == l.ReferenceCode);
                string? partnerName = null;
                if (transfer != null)
                {
                    if (l.TransactionType == "TRANSFER_OUT")
                        partnerName = transfer.ToBranch?.Name;
                    else if (l.TransactionType == "TRANSFER_IN")
                        partnerName = transfer.FromBranch?.Name;
                }
                return new InventoryLedgerHistoryDTO
                {
                    Id = l.Id,
                    BranchId = l.BranchId,
                    BranchName = l.Branch?.Name ?? "N/A",
                    ProductId = l.ProductId,
                    ProductName = l.Product?.Name ?? "N/A",
                    ProductSku = l.Product?.Sku ?? "N/A",
                    TransactionType = l.TransactionType,
                    QuantityChange = l.QuantityChange,
                    ReferenceCode = l.ReferenceCode,
                    CreatedAt = l.CreatedAt,
                    CreatedBy = l.CreatedBy,
                    ApprovedBy = transfer?.ApprovedBy,
                    ApproverName = transfer?.ApprovedByNavigation?.FullName,
                    CreatorName = l.Creator?.FullName ?? "Hệ thống",
                    PartnerBranchName = partnerName
                };
            });
        }

        public IEnumerable<InventoryTransferHistoryDTO> GetInventoryTransferHistory(int? branchId = null)
        {
            var transfers = _transferRepo.GetAllTransfersWithDetails(branchId);

            return transfers.Select(t => new InventoryTransferHistoryDTO
            {
                Id = t.Id,
                TransferCode = t.TransferCode,
                FromBranchId = t.FromBranchId,
                FromBranchName = t.FromBranch?.Name ?? "N/A",
                ToBranchId = t.ToBranchId,
                ToBranchName = t.ToBranch?.Name ?? "N/A",
                Status = t.Status,
                Note = t.Note,
                CreatedAt = t.CreatedAt ?? DateTime.Now,
                CreatedBy = t.CreatedBy ?? 1,
                CreatorName = t.CreatedByNavigation?.FullName ?? "N/A",
                ApprovedAt = t.ApprovedAt,
                ApprovedBy = t.ApprovedBy,
                ApproverName = t.ApprovedByNavigation?.FullName,

                Items = t.InventoryTransferDetails.Select(d => new InventoryTransferDetailHistoryDTO
                {
                    ProductId = d.ProductId,
                    ProductName = d.Product?.Name ?? "N/A",
                    ProductSku = d.Product?.Sku ?? "N/A",
                    Quantity = d.Quantity
                }).ToList()
            });
        }

        public IEnumerable<InventoryLedgerGroupedDTO> GetInventoryLedgerGroupedHistory(int? branchId = null)
        {
            var ledgers = _ledgerRepo.GetLedgersWithDetails(branchId);

            var transfers = _transferRepo.GetAllTransfersWithDetails(null).ToList();
            var grouped = ledgers
                .Where(l => !string.IsNullOrEmpty(l.ReferenceCode))
                .GroupBy(l => new { l.ReferenceCode, l.BranchId, l.TransactionType })
                .Select(g =>
                {
                    var firstLedger = g.First();
                    string? partnerName = null;

                    var transfer = transfers.FirstOrDefault(t => t.TransferCode == g.Key.ReferenceCode);

                    if (g.Key.TransactionType == "TRANSFER_OUT" || g.Key.TransactionType == "TRANSFER_IN")
                    {

                        if (transfer != null)
                        {
                            if (g.Key.TransactionType == "TRANSFER_OUT")
                                partnerName = transfer.ToBranch?.Name;
                            else if (g.Key.TransactionType == "TRANSFER_IN")
                                partnerName = transfer.FromBranch?.Name;
                        }
                    }

                    return new InventoryLedgerGroupedDTO
                    {
                        ReferenceCode = g.Key.ReferenceCode,
                        TransactionType = g.Key.TransactionType,
                        BranchId = g.Key.BranchId,
                        BranchName = firstLedger.Branch?.Name ?? "N/A",
                        PartnerBranchName = partnerName,
                        CreatedAt = firstLedger.CreatedAt,
                        CreatorName = firstLedger.Creator?.FullName ?? "Hệ thống",
                        ApproverName = transfer?.ApprovedByNavigation?.FullName ?? (transfer != null ? "Chưa duyệt" : null),
                        ApprovedBy = transfer?.ApprovedBy,
                        TotalItems = g.Select(x => x.ProductId).Distinct().Count(),
                        TotalQuantity = g.Sum(x => Math.Abs(x.QuantityChange)),
                        Details = g.Select(l => new InventoryLedgerHistoryDTO
                        {
                            Id = l.Id,
                            ProductId = l.ProductId,
                            ProductName = l.Product?.Name ?? "N/A",
                            ProductSku = l.Product?.Sku ?? "N/A",
                            TransactionType = l.TransactionType,
                            QuantityChange = l.QuantityChange,
                            ReferenceCode = l.ReferenceCode,
                            ApproverName = transfer?.ApprovedByNavigation?.FullName
                        }).ToList()
                    };
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            return grouped;
        }
    }
}