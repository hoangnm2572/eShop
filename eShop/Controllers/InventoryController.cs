using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using BusinessObjects.DTOs;

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
        public IActionResult GetInventory()
        {
            try
            {
                var inventory = _inventoryService.GetInventory();
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("branch/{branchId}")]
        public IActionResult GetInventoryByBranch(int branchId)
        {
            try
            {
                var inventory = _inventoryService.GetInventoryByBranch(branchId);
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("branch/{branchId}/import")]
        public IActionResult ImportToBranch(int branchId, [FromBody] DirectImportRequestDTO request)
        {
            try
            {
                request.BranchId = branchId;
                _inventoryService.DirectImportToBranch(request);
                return Ok(new { message = "Nhập kho thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("branch/{branchId}/export")]
        public IActionResult ExportFromBranch(int branchId, [FromBody] DirectExportRequestDTO request)
        {
            try
            {
                request.BranchId = branchId;
                _inventoryService.DirectExportFromBranch(request);
                return Ok(new { message = "Xuất kho thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("transfer")]
        public IActionResult Transfer([FromBody] DirectTransferRequestDTO request)
        {
            try
            {
                if (request.FromBranchId == request.ToBranchId)
                    return BadRequest(new { message = "Kho xuất và kho nhập không được trùng nhau!" });

                _inventoryService.TransferInventory(request);
                return Ok(new { message = "Chuyển kho thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("branch/{branchId}/request-goods")]
        public IActionResult RequestGoods(int branchId, [FromBody] RequestGoodsDTO request)
        {
            try
            {
                _inventoryService.RequestGoodsFromHub(branchId, request);
                return Ok(new { message = "Đã gửi yêu cầu xin cấp hàng lên Kho tổng thành công. Vui lòng chờ duyệt!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("transfer/{transferId}/approve")]
        public IActionResult ApproveGoodsRequest(int transferId, [FromBody] ApproveGoodsRequestDTO request, [FromQuery] int userId)
        {
            try
            {
                _inventoryService.ApproveGoodsRequest(transferId, request, userId);
                return Ok(new { message = "Đã duyệt và chuyển sang đóng hàng thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("transfer/{transferId}/complete")]
        public IActionResult CompleteGoodsRequest(int transferId, [FromQuery] int userId)
        {
            try
            {
                _inventoryService.CompleteGoodsRequest(transferId, userId);
                return Ok(new { message = "Đã giao hàng và nhập kho thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("transfer/{transferId}/cancel")]
        public IActionResult CancelGoodsRequest(int transferId, [FromQuery] int userId)
        {
            try
            {
                _inventoryService.CancelGoodsRequest(transferId, userId);
                return Ok(new { message = "Đã hủy phiếu yêu cầu cấp hàng thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("branch/{branchId}/request-goods/{transferId}")]
        public IActionResult UpdateGoodsRequest(int branchId, int transferId, [FromBody] RequestGoodsDTO request)
        {
            try
            {
                _inventoryService.UpdateGoodsRequest(transferId, branchId, request);
                return Ok(new { message = "Đã cập nhật đơn xin hàng thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history/ledger")]
        public IActionResult GetLedgerHistory([FromQuery] int? branchId = null)
        {
            try
            {
                var result = _inventoryService.GetInventoryLedgerHistory(branchId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history/transfers")]
        public IActionResult GetTransferHistory([FromQuery] int? branchId = null)
        {
            try
            {
                var result = _inventoryService.GetInventoryTransferHistory(branchId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("history/ledger/grouped")]
        public IActionResult GetLedgerGroupedHistory([FromQuery] int? branchId = null)
        {
            try
            {
                var result = _inventoryService.GetInventoryLedgerGroupedHistory(branchId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}