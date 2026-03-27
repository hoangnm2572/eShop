using BusinessObjects;
using BusinessObjects.DTOs;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IInventoryLedgerRepository _ledgerRepo;
        private readonly IInventoryTransferRepository _transferRepo;
        private readonly IInventoryTransferDetailRepository _transferDetailRepo;
        private readonly IBranchRepository _branchRepo;

        public InventoryService(
            IInventoryRepository inventoryRepo,
            IInventoryLedgerRepository ledgerRepo,
            IInventoryTransferRepository transferRepo,
            IInventoryTransferDetailRepository transferDetailRepo,
            IBranchRepository branchRepo)
        {
            _inventoryRepo = inventoryRepo;
            _ledgerRepo = ledgerRepo;
            _transferRepo = transferRepo;
            _transferDetailRepo = transferDetailRepo;
            _branchRepo = branchRepo;
        }

        public async Task<PagedResponseDTO<InventoryResponseDTO>> GetInventoryByBranchAsync(int branchId, string? searchTerm = null, int? productGroupId = null, int? supplierId = null, int page = 1, int pageSize = 10, bool inStockOnly = false)
        {
            return await GetInventoryInternalAsync(branchId, searchTerm, productGroupId, supplierId, page, pageSize,inStockOnly);
        }

        public async Task<PagedResponseDTO<InventoryResponseDTO>> GetInventoryAsync(string? searchTerm = null, int? productGroupId = null, int? supplierId = null, int page = 1, int pageSize = 10, bool inStockOnly = false)
        {
            return await GetInventoryInternalAsync(null, searchTerm, productGroupId, supplierId, page, pageSize, inStockOnly);
        }

        private async Task<PagedResponseDTO<InventoryResponseDTO>> GetInventoryInternalAsync(int? branchId, string? searchTerm, int? productGroupId, int? supplierId, int page, int pageSize,bool inStockOnly = false)
        {
            if (!branchId.HasValue)
            {
                var branches = await _branchRepo.GetAllAsync();
                var warehouse = branches.FirstOrDefault(b => b.BranchType == "WAREHOUSE");
                if (warehouse != null)
                {
                    branchId = warehouse.Id;
                }
            }

            var (inventories, totalCount) = await _inventoryRepo.GetPagedInventoryWithDetailsAsync(branchId, searchTerm, productGroupId, supplierId, page, pageSize, inStockOnly);

            var items = inventories.Select(i => new InventoryResponseDTO
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Barcode = i.Product?.Barcode,
                Sku = i.Product?.Sku ?? string.Empty,
                Name = i.Product?.Name ?? string.Empty,
                BaseUnit = i.Product?.BaseUnit ?? string.Empty,
                PurchasePrice = i.Product?.PurchasePrice ?? 0,
                SalePrice = i.Product?.SalePrice ?? 0,
                IsActive = i.Product?.IsActive ?? false,
                ShowOnPos = i.Product?.ShowOnPos ?? false,
                ProductGroupName = i.Product?.ProductGroup?.Name,
                SupplierName = i.Product?.Supplier?.Name,
                UnitConversions = i.Product?.UnitConversions?.Select(u => new UnitConversionDTO
                {
                    Id = u.Id,
                    UnitName = u.UnitName,
                    ConversionRate = u.ConversionRate,
                    PurchasePrice = u.PurchasePrice,
                    SalePrice = u.SalePrice
                }).ToList() ?? new List<UnitConversionDTO>()
            }).ToList();

            return new PagedResponseDTO<InventoryResponseDTO>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task DirectImportToBranchAsync(DirectImportRequestDTO request)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            string refCode = $"IMP_{DateTime.Now:yyyyMMddHHmmss}";
            foreach (var item in request.Items)
            {
                var inv = await _inventoryRepo.GetInventoryByBranchAndProductAsync(request.BranchId, item.ProductId);
                if (inv == null)
                {
                    inv = new Inventory { BranchId = request.BranchId, ProductId = item.ProductId, Quantity = item.Quantity };
                    await _inventoryRepo.AddAsync(inv);
                }
                else
                {
                    inv.Quantity += item.Quantity;
                    await _inventoryRepo.UpdateAsync(inv);
                }

                var ledger = new InventoryLedger
                {
                    BranchId = request.BranchId,
                    ProductId = item.ProductId,
                    TransactionType = "IMPORT",
                    QuantityChange = item.Quantity,
                    ReferenceCode = refCode,
                    CreatedAt = DateTime.Now,
                    CreatedBy = request.CreatedBy
                };
                await _ledgerRepo.AddAsync(ledger);
            }
            scope.Complete();
        }

        public async Task DirectExportFromBranchAsync(DirectExportRequestDTO request)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            string refCode = $"EXP_{DateTime.Now:yyyyMMddHHmmss}";
            foreach (var item in request.Items)
            {
                var inv = await _inventoryRepo.GetInventoryByBranchAndProductAsync(request.BranchId, item.ProductId);
                if (inv == null || inv.Quantity < item.Quantity)
                    throw new Exception($"Sản phẩm có ID {item.ProductId} không đủ số lượng trong kho!");

                inv.Quantity -= item.Quantity;
                await _inventoryRepo.UpdateAsync(inv);

                var ledger = new InventoryLedger
                {
                    BranchId = request.BranchId,
                    ProductId = item.ProductId,
                    TransactionType = "EXPORT",
                    QuantityChange = -item.Quantity,
                    ReferenceCode = refCode,
                    CreatedAt = DateTime.Now,
                    CreatedBy = request.CreatedBy
                };
                await _ledgerRepo.AddAsync(ledger);
            }
            scope.Complete();
        }

        public async Task TransferInventoryAsync(DirectTransferRequestDTO request)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            string refCode = $"TRF_{DateTime.Now:yyyyMMddHHmmss}";

            var transfer = new InventoryTransfer
            {
                TransferCode = refCode,
                FromBranchId = request.FromBranchId,
                ToBranchId = request.ToBranchId,
                Status = "COMPLETED",
                Note = "Luân chuyển trực tiếp",
                CreatedAt = DateTime.Now,
                CreatedBy = request.CreatedBy,
                ApprovedAt = DateTime.Now,
                ApprovedBy = request.CreatedBy
            };
            await _transferRepo.AddAsync(transfer);

            foreach (var item in request.Items)
            {
                var fromInv = await _inventoryRepo.GetInventoryByBranchAndProductAsync(request.FromBranchId, item.ProductId);
                if (fromInv == null || fromInv.Quantity < item.Quantity)
                    throw new Exception($"Sản phẩm có ID {item.ProductId} không đủ số lượng tại kho xuất!");

                fromInv.Quantity -= item.Quantity;
                await _inventoryRepo.UpdateAsync(fromInv);

                var ledgerExport = new InventoryLedger
                {
                    BranchId = request.FromBranchId,
                    ProductId = item.ProductId,
                    TransactionType = "TRANSFER_OUT",
                    QuantityChange = -item.Quantity,
                    ReferenceCode = refCode,
                    CreatedAt = DateTime.Now,
                    CreatedBy = request.CreatedBy
                };
                await _ledgerRepo.AddAsync(ledgerExport);

                var toInv = await _inventoryRepo.GetInventoryByBranchAndProductAsync(request.ToBranchId, item.ProductId);
                if (toInv == null)
                {
                    toInv = new Inventory { BranchId = request.ToBranchId, ProductId = item.ProductId, Quantity = item.Quantity };
                    await _inventoryRepo.AddAsync(toInv);
                }
                else
                {
                    toInv.Quantity += item.Quantity;
                    await _inventoryRepo.UpdateAsync(toInv);
                }

                var ledgerImport = new InventoryLedger
                {
                    BranchId = request.ToBranchId,
                    ProductId = item.ProductId,
                    TransactionType = "TRANSFER_IN",
                    QuantityChange = item.Quantity,
                    ReferenceCode = refCode,
                    CreatedAt = DateTime.Now,
                    CreatedBy = request.CreatedBy
                };
                await _ledgerRepo.AddAsync(ledgerImport);
            }
            scope.Complete();
        }

        public async Task RequestGoodsFromHubAsync(int requestingBranchId, RequestGoodsDTO request)
        {
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            string refCode = $"REQ_{DateTime.Now:yyyyMMddHHmmss}";

            var branches = await _branchRepo.GetAllAsync();
            var hub = branches.FirstOrDefault(b => b.BranchType == "WAREHOUSE");
            if (hub == null) throw new Exception("Không tìm thấy Kho Tổng!");

            var transfer = new InventoryTransfer
            {
                TransferCode = refCode,
                FromBranchId = hub.Id,
                ToBranchId = requestingBranchId,
                Status = "PENDING",
                Note = request.Note,
                CreatedAt = DateTime.Now,
                CreatedBy = request.CreatedBy
            };
            await _transferRepo.AddAsync(transfer);

            foreach (var item in request.Items)
            {
                var detail = new InventoryTransferDetail
                {
                    TransferId = transfer.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                await _transferDetailRepo.AddAsync(detail);
            }
            scope.Complete();
        }

        public async Task ApproveGoodsRequestAsync(int transferId, ApproveGoodsRequestDTO request, int userId)
        {
            var transfer = await _transferRepo.GetByIdAsync(transferId);
            if (transfer == null) throw new Exception("Không tìm thấy phiếu yêu cầu!");
            if (transfer.Status != "PENDING") throw new Exception("Trạng thái phiếu không hợp lệ để duyệt!");

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var existingDetails = await _transferDetailRepo.GetByTransferIdAsync(transferId);
            foreach (var ed in existingDetails)
            {
                await _transferDetailRepo.DeleteAsync(ed);
            }

            foreach (var item in request.ItemsToApprove)
            {
                var fromInv = await _inventoryRepo.GetInventoryByBranchAndProductAsync(transfer.FromBranchId, item.ProductId);
                if (fromInv == null || fromInv.Quantity < item.Quantity)
                    throw new Exception($"Không đủ hàng cho sản phẩm ID {item.ProductId} tại kho tổng!");

                fromInv.Quantity -= item.Quantity;
                await _inventoryRepo.UpdateAsync(fromInv);

                var ledgerOut = new InventoryLedger
                {
                    BranchId = transfer.FromBranchId,
                    ProductId = item.ProductId,
                    TransactionType = "TRANSFER_OUT",
                    QuantityChange = -item.Quantity,
                    ReferenceCode = transfer.TransferCode,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId
                };
                await _ledgerRepo.AddAsync(ledgerOut);

                var newDetail = new InventoryTransferDetail
                {
                    TransferId = transfer.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                await _transferDetailRepo.AddAsync(newDetail);
            }

            transfer.Status = "SHIPPING";
            transfer.ApprovedAt = DateTime.Now;
            transfer.ApprovedBy = userId;
            await _transferRepo.UpdateAsync(transfer);

            scope.Complete();
        }

        public async Task CancelGoodsRequestAsync(int transferId, int userId)
        {
            var transfer = await _transferRepo.GetByIdAsync(transferId);
            if (transfer == null) throw new Exception("Không tìm thấy phiếu!");
            if (transfer.Status != "PENDING") throw new Exception("Chỉ có thể hủy phiếu đang chờ duyệt!");

            transfer.Status = "CANCELLED";
            transfer.ApprovedAt = DateTime.Now;
            transfer.ApprovedBy = userId;
            await _transferRepo.UpdateAsync(transfer);
        }

        public async Task CompleteGoodsRequestAsync(int transferId, int userId)
        {
            var transfer = await _transferRepo.GetByIdAsync(transferId);
            if (transfer == null) throw new Exception("Không tìm thấy phiếu!");
            if (transfer.Status != "SHIPPING") throw new Exception("Chỉ có thể hoàn thành phiếu đang giao!");

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var details = await _transferDetailRepo.GetByTransferIdAsync(transferId);
            foreach (var d in details)
            {
                var toInv = await _inventoryRepo.GetInventoryByBranchAndProductAsync(transfer.ToBranchId, d.ProductId);
                if (toInv == null)
                {
                    toInv = new Inventory { BranchId = transfer.ToBranchId, ProductId = d.ProductId, Quantity = d.Quantity };
                    await _inventoryRepo.AddAsync(toInv);
                }
                else
                {
                    toInv.Quantity += d.Quantity;
                    await _inventoryRepo.UpdateAsync(toInv);
                }

                var ledgerIn = new InventoryLedger
                {
                    BranchId = transfer.ToBranchId,
                    ProductId = d.ProductId,
                    TransactionType = "TRANSFER_IN",
                    QuantityChange = d.Quantity,
                    ReferenceCode = transfer.TransferCode,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId
                };
                await _ledgerRepo.AddAsync(ledgerIn);
            }

            transfer.Status = "COMPLETED";
            await _transferRepo.UpdateAsync(transfer);

            scope.Complete();
        }

        public async Task UpdateGoodsRequestAsync(int transferId, int branchId, RequestGoodsDTO request)
        {
            var transfer = await _transferRepo.GetByIdAsync(transferId);
            if (transfer == null) throw new Exception("Không tìm thấy phiếu!");
            if (transfer.ToBranchId != branchId) throw new Exception("Phiếu không thuộc chi nhánh này!");
            if (transfer.Status != "PENDING") throw new Exception("Chỉ có thể cập nhật phiếu đang chờ duyệt!");

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            transfer.Note = request.Note;
            await _transferRepo.UpdateAsync(transfer);

            var existingDetails = await _transferDetailRepo.GetByTransferIdAsync(transferId);
            foreach (var d in existingDetails)
            {
                await _transferDetailRepo.DeleteAsync(d);
            }

            foreach (var item in request.Items)
            {
                var detail = new InventoryTransferDetail
                {
                    TransferId = transfer.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                };
                await _transferDetailRepo.AddAsync(detail);
            }

            scope.Complete();
        }

        public async Task<PagedResponseDTO<InventoryLedgerHistoryDTO>> GetInventoryLedgerHistoryAsync(
            int? branchId = null, string? searchTerm = null, string? transactionType = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10)
        {
            var (ledgers, totalCount) = await _ledgerRepo.GetPagedLedgersWithDetailsAsync(branchId, searchTerm, transactionType, startDate, endDate, page, pageSize);

            var refCodes = ledgers.Where(l => !string.IsNullOrEmpty(l.ReferenceCode))
                                  .Select(l => l.ReferenceCode)
                                  .Distinct()
                                  .ToList();

            var transfers = await _transferRepo.GetTransfersByCodesAsync(refCodes);

            var items = ledgers.Select(l => {
                var transfer = transfers.FirstOrDefault(t => t.TransferCode == l.ReferenceCode);
                string partnerName = null;

                if (transfer != null)
                {
                    partnerName = l.BranchId == transfer.FromBranchId
                        ? (transfer.ToBranch?.Name ?? "Kho tổng")
                        : (transfer.FromBranch?.Name ?? "Kho tổng");
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
                    CreatorName = l.Creator?.FullName ?? "Hệ thống",
                    PartnerBranchName = partnerName
                };
            }).ToList();

            return new PagedResponseDTO<InventoryLedgerHistoryDTO> { Items = items, TotalCount = totalCount, Page = page, PageSize = pageSize };
        }

        public async Task<PagedResponseDTO<InventoryTransferHistoryDTO>> GetInventoryTransferHistoryAsync(
            int? branchId = null, string? searchTerm = null, string? status = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10, bool isRequestOnly = false)
        {
            var (transfers, totalCount) = await _transferRepo.GetPagedTransfersWithDetailsAsync(branchId, searchTerm, status, startDate, endDate, page, pageSize, isRequestOnly);

            var items = transfers.Select(t => new InventoryTransferHistoryDTO
            {
                Id = t.Id,
                TransferCode = t.TransferCode,
                FromBranchId = t.FromBranchId,
                FromBranchName = t.FromBranch?.Name ?? "N/A",
                ToBranchId = t.ToBranchId,
                ToBranchName = t.ToBranch?.Name ?? "N/A",
                Status = t.Status,
                Note = t.Note,
                CreatedAt = t.CreatedAt ?? DateTime.MinValue,
                CreatedBy = t.CreatedBy ?? 0,
                CreatorName = t.CreatedByNavigation?.FullName ?? "Hệ thống",
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
            }).ToList();

            return new PagedResponseDTO<InventoryTransferHistoryDTO> { Items = items, TotalCount = totalCount, Page = page, PageSize = pageSize };
        }

        public async Task<PagedResponseDTO<InventoryLedgerGroupedDTO>> GetInventoryLedgerGroupedHistoryAsync(
            int? branchId = null, string? searchTerm = null, string? transactionType = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 10)
        {
            var (ledgers, totalGroups) = await _ledgerRepo.GetPagedGroupedLedgersAsync(branchId, searchTerm, transactionType, startDate, endDate, page, pageSize);

            var refCodes = ledgers.Select(l => l.ReferenceCode).Distinct().ToList();
            var transfers = await _transferRepo.GetTransfersByCodesAsync(refCodes);

            var groupedItems = ledgers
                .GroupBy(l => new { l.ReferenceCode, l.BranchId, l.TransactionType })
                .Select(g =>
                {
                    var firstLedger = g.First();
                    var transfer = transfers.FirstOrDefault(t => t.TransferCode == firstLedger.ReferenceCode);
                    string partnerName = null;

                    if (transfer != null)
                    {
                        partnerName = firstLedger.BranchId == transfer.FromBranchId
                            ? (transfer.ToBranch?.Name ?? "Kho tổng")
                            : (transfer.FromBranch?.Name ?? "Kho tổng");
                    }

                    return new InventoryLedgerGroupedDTO
                    {
                        ReferenceCode = firstLedger.ReferenceCode ?? "N/A",
                        TransactionType = firstLedger.TransactionType,
                        BranchId = firstLedger.BranchId,
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

            return new PagedResponseDTO<InventoryLedgerGroupedDTO> { Items = groupedItems, TotalCount = totalGroups, Page = page, PageSize = pageSize };
        }
    }
}