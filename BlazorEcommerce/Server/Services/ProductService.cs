
using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Services
{
    public interface IProductService
    {
        Task<ServiceResponse<List<Product>>> GetProductAsync();
        Task<ServiceResponse<Product>> GetProductAsync(int productId);

    }

    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductAsync()
        {
            var response = new ServiceResponse<List<Product>>();

            response.Data = await _context.Products.ToListAsync();

            return response;
        }

        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();

            response.Data = await _context.Products.FirstOrDefaultAsync(q => q.Id == productId);

            if (response.Data == null) {
                response.Success = false;
                response.Message = "Product not Found !!";
            }

            return response;
        }
    }
}
