using BlazorEcommerce.Shared;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace BlazorEcommerce.Client.Services
{
    public interface ICartService
    {
        event Action OnChange;
        Task AddToCart(CartItem cartItem);
        Task<List<CartItem>> GetCartItems();
        Task<List<CartProductResponse>> GetCartPorducts();
        Task RemoveProductFromCart(int productId, int productTypeId);
        Task UpdateQty(CartProductResponse productResponse);
    }

    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _httpClient;

        public CartService(ILocalStorageService localStorage, HttpClient httpClient)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
        }

        public event Action OnChange;

        public async Task AddToCart(CartItem cartItem)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            
            if(cart == null)
            {
                cart = new List<CartItem>();
            }

            var sameItem = cart.Find(q => q.ProductId == cartItem.ProductId &&
                            q.ProductTypeId == cartItem.ProductTypeId);

            if(sameItem == null)
            {
                cart.Add(cartItem);
            } else
            {
                sameItem.Qty += cartItem.Qty;
            }
            

            await _localStorage.SetItemAsync("cart", cart);

            OnChange.Invoke();
        }

        public async Task<List<CartItem>> GetCartItems()
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");

            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            return cart;
        }

        public async Task<List<CartProductResponse>> GetCartPorducts()
        {
            var cartItem = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            var response = await _httpClient.PostAsJsonAsync("api/cart/products", cartItem);
            var cartProduct = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

            return cartProduct.Data;
        }

        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if(cart == null)
            {
                return;
            }

            var cartItem = cart.Find(q => q.ProductId == productId && q.ProductTypeId == productTypeId);
            
            if(cartItem != null)
            {
                cart.Remove(cartItem);
                await _localStorage.SetItemAsync<List<CartItem>>("cart", cart);
                OnChange.Invoke();
            }
        }

        public async Task UpdateQty(CartProductResponse productResponse)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
            {
                return;
            }

            var cartItem = cart.Find(q => q.ProductId == productResponse.ProductId && q.ProductTypeId == productResponse.ProductTypeId);

            if (cartItem != null)
            {
                cartItem.Qty = productResponse.Qty;
                await _localStorage.SetItemAsync<List<CartItem>>("cart", cart);
            }
        }
    }
}
