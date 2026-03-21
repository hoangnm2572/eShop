using Microsoft.AspNetCore.Mvc;
using System.Linq;
using BussinessObjects;
using Services;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController()
        {
            _productService = new ProductService();
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
        public IActionResult Create([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _productService.SaveProduct(product);
            return Ok(new { message = "Thêm sản phẩm thành công!" });
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest(new { message = "ID sản phẩm không khớp" });
            }

            _productService.UpdateProduct(product);
            return Ok(new { message = "Cập nhật thành công!" });
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
    }
}