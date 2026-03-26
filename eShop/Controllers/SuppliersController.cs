using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var suppliers = await _supplierService.GetAllSuppliersAsync();
                return Ok(suppliers);
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
                var supplier = await _supplierService.GetSupplierByIdAsync(id);
                return Ok(supplier);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SupplierRequestDTO dto)
        {
            try
            {
                await _supplierService.SaveSupplierAsync(dto);
                return Ok(new { message = "Thêm Nhà cung cấp thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SupplierRequestDTO dto)
        {
            try
            {
                await _supplierService.UpdateSupplierAsync(id, dto);
                return Ok(new { message = "Cập nhật Nhà cung cấp thành công" });
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
                await _supplierService.DeleteSupplierAsync(id);
                return Ok(new { message = "Xóa Nhà cung cấp thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}