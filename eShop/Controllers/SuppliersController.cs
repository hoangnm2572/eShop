using BusinessObjects;
using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var suppliers = _supplierService.GetAllSuppliers();
            return Ok(suppliers);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var supplier = _supplierService.GetSupplierById(id);
                return Ok(supplier);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] SupplierRequestDTO dto)
        {
            _supplierService.SaveSupplier(dto);
            return Ok(new { message = "Thêm Nhà cung cấp thành công" });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] SupplierRequestDTO dto)
        {
            _supplierService.UpdateSupplier(id, dto);
            return Ok(new { message = "Cập nhật Nhà cung cấp thành công" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _supplierService.DeleteSupplier(id);
                return Ok(new { message = "Xóa Nhà cung cấp thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
