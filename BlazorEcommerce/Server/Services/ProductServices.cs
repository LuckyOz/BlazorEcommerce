
using Microsoft.EntityFrameworkCore;

namespace BlazorEcommerce.Server.Services
{
    public interface IProductServices
    {
        Task<ServiceResponse<List<Product>>> GetProductAsync();
        Task<ServiceResponse<Product>> GetProductAsync(int productId);
        Task<ServiceResponse<List<Product>>> GetProductByCategory(string categoryUrl);
        Task<ServiceResponse<ProductSearchResult>> SearchProducts(string searchText, int page);
        Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText);
        Task<ServiceResponse<List<Product>>> GetFeaturedProducts();
    }

    public class ProductServices : IProductServices
    {
        private readonly DataContext _context;

        public ProductServices(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
                    .Where(q => q.Featured)
                    .Include(q => q.Variants)
                    .ToListAsync()
            };

            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductAsync()
        {
            var response = new ServiceResponse<List<Product>>();

            response.Data = await _context.Products
                .Include(q => q.Variants)
                .ThenInclude(q => q.ProductType)
                .ToListAsync();

            return response;
        }

        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();

            response.Data = await _context.Products
                .Include(q => q.Variants)
                .ThenInclude(q => q.ProductType)
                .FirstOrDefaultAsync(q => q.Id == productId);

            if (response.Data == null) {
                response.Success = false;
                response.Message = "Product not Found !!";
            }

            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductByCategory(string categoryUrl)
        {
            var response = new ServiceResponse<List<Product>>();
            response.Data = await _context.Products
                .Where(q => q.Category!.Url!.ToLower().Equals(categoryUrl.ToLower()))
                .Include(q => q.Variants)
                .ThenInclude(q => q.ProductType)
                .ToListAsync();

            return response;
        }

        public async Task<ServiceResponse<List<string>>> GetProductSearchSuggestions(string searchText)
        {
            var products = await FindProductsBySearchText(searchText);

            List<string> result = new List<string>();

            foreach (var product in products)
            {
                if(product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(product.Title);
                }

                if(product.Description != null)
                {
                    var punctuation = product.Description.Where(char.IsPunctuation)
                       .Distinct().ToArray();
                    var words = product.Description.Split()
                        .Select(q => q.Trim(punctuation));

                    foreach (var word in words)
                    {
                        if (word.Contains(searchText, StringComparison.OrdinalIgnoreCase) && !result.Contains(word))
                        {
                            result.Add(word);
                        }
                    }
                }
            }

            return new ServiceResponse<List<string>> { Data = result };
        }

        public async Task<ServiceResponse<ProductSearchResult>> SearchProducts(string searchText, int page)
        {
            var pageResult = 2f;
            var pageCount = Math.Ceiling((await FindProductsBySearchText(searchText)).Count / pageResult);
            var product = await _context.Products
                            .Where(q => q.Title.ToLower().Contains(searchText.ToLower())
                                    || q.Description.ToLower().Contains(searchText.ToLower()))
                            .Include(q => q.Variants)
                            .Skip((page - 1) * (int)pageResult)
                            .Take((int)pageResult)
                            .ToListAsync();

            var response = new ServiceResponse<ProductSearchResult>
            {
                Data = new ProductSearchResult
                {
                    Products = product,
                    CurrentPage = page,
                    Pages = (int)pageCount
                }
            };

            return response;;
        }

        private async Task<List<Product>> FindProductsBySearchText(string searchText)
        {
            return await _context.Products
                .Where(q => q.Title.ToLower().Contains(searchText.ToLower())
                        || q.Description.ToLower().Contains(searchText.ToLower()))
                .Include(q => q.Variants)
                .ToListAsync();
        }
    }
}
