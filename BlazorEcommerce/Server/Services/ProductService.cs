
namespace BlazorEcommerce.Server.Services
{
    public interface IProductService
    {
        Task<ServiceResponse<List<Product>>> GetProductAsync();
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
    }
}
