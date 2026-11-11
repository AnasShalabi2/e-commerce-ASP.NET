using KASHOP.BLL.Dto;
using KASHOP.BLL.Services;
using KASHOP.DAL.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KASHOP.PL.Controllers
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
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateProduct([FromForm] ProductRequest request) {
            var result = await _productService.CreateProduct(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("")]
        public async Task<ActionResult> GetAllProducts([FromQuery] ProductQueryRequest request)
        {
            var result = await _productService.GetAllProducts(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetOne(int id)
        {
            var result = await _productService.GetProduct(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProduct(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
