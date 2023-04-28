using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryServices _categoryServices;

        public CategoryController(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Category>>>> GetCategories()
        {

            var result = new ServiceResponse<List<Category>>();

            try
            {
                result = await _categoryServices.GetCategories();
            } catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            
            return Ok(result);
        }
    }
}
