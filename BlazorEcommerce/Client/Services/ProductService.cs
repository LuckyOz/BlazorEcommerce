
using System.Net.Http.Json;
using System.Security;

namespace BlazorEcommerce.Client.Services
{
    public interface IProductService
    {
        event Action ProductChanged;
        
        int PageCount { get; set; }
        int CurrentPage { get; set; }
        string Message { get; set; }
        string LastSearchText { get; set; }

        List<Product> Products { get; set; }

        Task GetProduct(string catergoryUrl = null!);
        Task<ServiceResponse<Product>> GetProduct(int productId);
        Task SearchProducts(string searchText, int page);
        Task<List<string>> GetProductSearchSuggestions(string searchText);
    }

    public class ProductService : IProductService
    {
        private readonly HttpClient _http;

        public ProductService(HttpClient http)
        {
            _http = http;
        }

        public List<Product> Products { get ; set; } = new List<Product>();
        public string Message { get; set; } = "Loading Products..";
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public string LastSearchText { get; set; } = string.Empty;

        public event Action ProductChanged;

        public async Task GetProduct(string catergoryUrl = null!)
        {
            var result = catergoryUrl == null ?
                await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/Product/featured") :
                await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/Product/category/{catergoryUrl}");

            if (result != null && result.Data != null)
                Products = result.Data;

            CurrentPage = 1;
            PageCount = 0;

            if (Products.Count == 0)
                Message = "No Product Found!!";

            ProductChanged.Invoke();
        }

        public async Task<ServiceResponse<Product>> GetProduct(int productId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<Product>>($"api/Product/{productId}");
            return result;
        }

        public async Task<List<string>> GetProductSearchSuggestions(string searchText)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<string>>>($"api/product/searchsuggestion/{searchText}");
            return result.Data;
        }

        public async Task SearchProducts(string searchText, int page)
        {
            LastSearchText = searchText;    
            var result = await _http.GetFromJsonAsync<ServiceResponse<ProductSearchResult>>($"api/product/search/{searchText}/{page}");
            if(result != null && result.Data != null)
            {
                Products = result.Data.Products;
                CurrentPage = result.Data.CurrentPage;
                PageCount = result.Data.Pages;
            }

            if(Products.Count == 0)
            {
                Message = "No Product found.";
            }

            ProductChanged.Invoke();
        }
    }
}
