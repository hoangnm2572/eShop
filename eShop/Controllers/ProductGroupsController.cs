using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductGroupsController : ControllerBase
    {
        private readonly IProductGroupService _productGroupService;

        public ProductGroupsController(IProductGroupService productGroupService)
        {
            _productGroupService = productGroupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var groups = await _productGroupService.GetAllProductGroupsAsync();
                return Ok(groups);
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
                var group = await _productGroupService.GetProductGroupByIdAsync(id);
                return Ok(group);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductGroupRequestDTO request)
        {
            try
            {
                await _productGroupService.SaveProductGroupAsync(request);
                return Ok(new { message = "Thêm Nhóm hàng hóa thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductGroupRequestDTO request)
        {
            try
            {
                await _productGroupService.UpdateProductGroupAsync(id, request);
                return Ok(new { message = "Cập nhật Nhóm hàng hóa thành công" });
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
                await _productGroupService.DeleteProductGroupAsync(id);
                return Ok(new { message = "Xóa Nhóm hàng hóa thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}