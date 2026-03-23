using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

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
        public IActionResult GetAll([FromQuery] bool onlyActive = true)
        {
            try
            {
                var branches = _branchService.GetAllBranches(onlyActive);
                return Ok(branches);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var branch = _branchService.GetBranchById(id);
            if (branch == null) return NotFound(new { message = "Chi nhánh không tồn tại" });
            return Ok(branch);
        }

        [HttpPost]
        public IActionResult Create([FromBody] BranchCreateDTO request)
        {
            try
            {
                _branchService.CreateBranch(request);
                return Ok(new { message = "Tạo chi nhánh thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] BranchUpdateDTO request)
        {
            try
            {
                _branchService.UpdateBranch(id, request);
                return Ok(new { message = "Cập nhật chi nhánh thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _branchService.DeleteBranch(id);
                return Ok(new { message = "Đã đóng cửa chi nhánh thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
