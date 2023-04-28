
namespace BlazorEcommerce.Server.Services
{
    public interface ICategoryServices
    {
        Task<ServiceResponse<List<Category>>> GetCategories();
    }

    public class CategoryServices : ICategoryServices
    {
        private readonly DataContext _context;

        public CategoryServices(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Category>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            return new ServiceResponse<List<Category>>
            {
                Data = categories
            };
        }
    }
}
