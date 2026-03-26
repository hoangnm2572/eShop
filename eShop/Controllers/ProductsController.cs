using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace eShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] int? productGroupId = null,
            [FromQuery] int? supplierId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] bool? showOnPos = null)
        {
            try
            {
                var result = await _productService.GetProductsAsync(
                    page, pageSize, search, productGroupId, supplierId, isActive, showOnPos
                );

                return Ok(result);
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
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm" });
            }
        }

        [HttpGet("barcode/{barcode}")]
        public async Task<IActionResult> GetByBarcode(string barcode)
        {
            try
            {
                var product = await _productService.GetProductByBarcodeAsync(barcode);
                return Ok(product);
            }
            catch
            {
                return NotFound(new { message = "Mã vạch này chưa được khai báo trong hệ thống!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductRequestDTO dto)
        {
            try
            {
                await _productService.SaveProductAsync(dto);
                return Ok(new { message = "Thêm sản phẩm thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductRequestDTO dto)
        {
            try
            {
                await _productService.UpdateProductAsync(id, dto);
                return Ok(new { message = "Cập nhật sản phẩm thành công" });
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
                await _productService.DeleteProductAsync(id);
                return Ok(new { message = "Xóa sản phẩm thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}