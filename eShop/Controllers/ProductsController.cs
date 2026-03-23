using BusinessObjects;
using BusinessObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;
using Services.Interfaces;

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
        public IActionResult GetAll()
        {
            var products = _productService.GetProducts();
            return Ok(products.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm" });
            }
            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProductRequestDTO dto)
        {
            _productService.SaveProduct(dto);
            return Ok(new { message = "Thêm sản phẩm thành công" });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ProductRequestDTO dto)
        {
            _productService.UpdateProduct(id, dto);
            return Ok(new { message = "Cập nhật sản phẩm thành công" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existingProduct = _productService.GetProductById(id);
            if (existingProduct == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm để xóa" });
            }

            _productService.DeleteProduct(id);
            return Ok(new { message = "Xóa sản phẩm thành công!" });
        }

        [HttpGet("barcode/{barcode}")]
        public IActionResult GetByBarcode(string barcode)
        {
            var product = _productService.GetProductByBarcode(barcode);

            if (product == null)
            {
                return NotFound(new { message = "Mã vạch này chưa được khai báo trong hệ thống!" });
            }

            return Ok(product);
        }
    }
}