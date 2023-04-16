
using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services
{
    public interface IProductService
    {
        List<Product> Products { get; set; }
        Task GetProduct();
        
    }

    public class ProductService : IProductService
    {
        private readonly HttpClient _http;

        public ProductService(HttpClient http)
        {
            _http = http;
        }

        public List<Product> Products { get ; set; } = new List<Product>();

        public async Task GetProduct()
        {
            var result = 
                await _http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/Product");

            if (result != null && result.Data != null)
                Products = result.Data;
        }
    }
}
