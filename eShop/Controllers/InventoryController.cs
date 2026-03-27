using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using BusinessObjects.DTOs;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetInventory(
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? productGroupId = null,
            [FromQuery] int? supplierId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var inventory = await _inventoryService.GetInventoryAsync(searchTerm, productGroupId, supplierId, page, pageSize);
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("branch/{branchId}")]
        public async Task<IActionResult> GetInventoryByBranch(
            int branchId,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? productGroupId = null,
            [FromQuery] int? supplierId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool inStockOnly = false)
        {
            try
            {
                var inventory = await _inventoryService.GetInventoryByBranchAsync(branchId, searchTerm, productGroupId, supplierId, page, pageSize,inStockOnly);
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("branch/{branchId}/import")]
        public async Task<IActionResult> ImportToBranch(int branchId, [FromBody] DirectImportRequestDTO request)
        {
            try
            {
                request.BranchId = branchId;
                await _inventoryService.DirectImportToBranchAsync(request);
                return Ok(new { message = "Nhập kho thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("branch/{branchId}/export")]
        public async Task<IActionResult> ExportFromBranch(int branchId, [FromBody] DirectExportRequestDTO request)
        {
            try
            {
                request.BranchId = branchId;
                await _inventoryService.DirectExportFromBranchAsync(request);
                return Ok(new { message = "Xuất kho thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] DirectTransferRequestDTO request)
        {
            try
            {
                if (request.FromBranchId == request.ToBranchId)
                    return BadRequest(new { message = "Kho xuất và kho nhập không được trùng nhau!" });

                await _inventoryService.TransferInventoryAsync(request);
                return Ok(new { message = "Chuyển kho thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("branch/{branchId}/request-goods")]
        public async Task<IActionResult> RequestGoods(int branchId, [FromBody] RequestGoodsDTO request)
        {
            try
            {
                await _inventoryService.RequestGoodsFromHubAsync(branchId, request);
                return Ok(new { message = "Đã gửi yêu cầu xin cấp hàng lên Kho tổng thành công. Vui lòng chờ duyệt!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("transfer/{transferId}/approve")]
        public async Task<IActionResult> ApproveGoodsRequest(int transferId, [FromBody] ApproveGoodsRequestDTO request, [FromQuery] int userId)
        {
            try
            {
                await _inventoryService.ApproveGoodsRequestAsync(transferId, request, userId);
                return Ok(new { message = "Đã duyệt và chuyển sang đóng hàng thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("transfer/{transferId}/complete")]
        public async Task<IActionResult> CompleteGoodsRequest(int transferId, [FromQuery] int userId)
        {
            try
            {
                await _inventoryService.CompleteGoodsRequestAsync(transferId, userId);
                return Ok(new { message = "Đã giao hàng và nhập kho thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("transfer/{transferId}/cancel")]
        public async Task<IActionResult> CancelGoodsRequest(int transferId, [FromQuery] int userId)
        {
            try
            {
                await _inventoryService.CancelGoodsRequestAsync(transferId, userId);
                return Ok(new { message = "Đã hủy phiếu yêu cầu cấp hàng thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("branch/{branchId}/request-goods/{transferId}")]
        public async Task<IActionResult> UpdateGoodsRequest(int branchId, int transferId, [FromBody] RequestGoodsDTO request)
        {
            try
            {
                await _inventoryService.UpdateGoodsRequestAsync(transferId, branchId, request);
                return Ok(new { message = "Đã cập nhật đơn xin hàng thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history/ledger")]
        public async Task<IActionResult> GetLedgerHistory(
            [FromQuery] int? branchId = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? typeFilter = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _inventoryService.GetInventoryLedgerHistoryAsync(branchId, searchTerm, typeFilter, startDate, endDate, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history/transfers")]
        public async Task<IActionResult> GetTransferHistory(
            [FromQuery] int? branchId = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? typeFilter = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool isRequestOnly = false)
        {
            try
            {
                var result = await _inventoryService.GetInventoryTransferHistoryAsync(branchId, searchTerm, typeFilter, startDate, endDate, page, pageSize, isRequestOnly);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history/ledger/grouped")]
        public async Task<IActionResult> GetLedgerGroupedHistory(
            [FromQuery] int? branchId = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? typeFilter = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _inventoryService.GetInventoryLedgerGroupedHistoryAsync(branchId, searchTerm, typeFilter, startDate, endDate, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}