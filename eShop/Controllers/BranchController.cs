using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = true)
        {
            try
            {
                var branches = await _branchService.GetAllBranchesAsync(onlyActive);
                return Ok(branches);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var branch = await _branchService.GetBranchByIdAsync(id);
                if (branch == null) return NotFound(new { message = "Chi nhánh không tồn tại" });
                return Ok(branch);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BranchCreateDTO request)
        {
            try
            {
                await _branchService.CreateBranchAsync(request);
                return Ok(new { message = "Tạo chi nhánh thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BranchUpdateDTO request)
        {
            try
            {
                await _branchService.UpdateBranchAsync(id, request);
                return Ok(new { message = "Cập nhật chi nhánh thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _branchService.DeleteBranchAsync(id);
                return Ok(new { message = "Đã đóng cửa chi nhánh thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}