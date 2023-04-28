using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services
{
    public interface IAuthServices
    {
        Task<ServiceResponse<int>> Register(UserRegister request);
    }

    public class AuthServices : IAuthServices
    {
        private readonly HttpClient _http;

        public AuthServices(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<int>> Register(UserRegister request)
        {
            var result = await _http.PostAsJsonAsync("api/auth/register", request);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
        }
    }
}
