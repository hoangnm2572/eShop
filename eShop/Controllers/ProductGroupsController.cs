using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

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
        public IActionResult GetAll()
        {
            var groups = _productGroupService.GetAllProductGroups();
            return Ok(groups);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var group = _productGroupService.GetProductGroupById(id);
                return Ok(group);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProductGroupRequestDTO request)
        {
            try
            {
                _productGroupService.SaveProductGroup(request);
                return Ok(new { message = "Thêm Nhóm hàng hóa thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ProductGroupRequestDTO request)
        {
            try
            {
                _productGroupService.UpdateProductGroup(id, request);
                return Ok(new { message = "Cập nhật Nhóm hàng hóa thành công" });
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
                _productGroupService.DeleteProductGroup(id);
                return Ok(new { message = "Xóa Nhóm hàng hóa thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
