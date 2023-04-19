
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProduct()
        {
            var response = new ServiceResponse<List<Product>>();

            try {
                response = await _productService.GetProductAsync();
            } catch (Exception ex) {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(int productId)
        {
            var response = new ServiceResponse<Product>();

            try {
                response = await _productService.GetProductAsync(productId);
            } catch (Exception ex) {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
