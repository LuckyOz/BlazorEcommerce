namespace BlazorEcommerce.Server.Services
{
    public interface ICartServices
    {
        Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems);
    }

    public class CartServices : ICartServices
    {
        private readonly DataContext _context;

        public CartServices(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems)
        {
            var result = new ServiceResponse<List<CartProductResponse>>
            {
                Data = new List<CartProductResponse>()
            };

            foreach (var item in cartItems)
            {
                var product = await _context.Products
                    .Where(q => q.Id == item.ProductId)
                    .FirstOrDefaultAsync();

                if(product == null)
                {
                    continue;
                }

                var productVariant = await _context.ProductVariants
                    .Where(q => q.ProductId == item.ProductId && q.ProductTypeId == item.ProductTypeId)
                    .Include(q => q.ProductType)
                    .FirstOrDefaultAsync();

                if(productVariant == null)
                {
                    continue;
                }

                var cartProduct = new CartProductResponse
                {
                    ProductId = product.Id,
                    Title = product.Title,
                    ImageUrl = product.ImageUrl,
                    Price = productVariant.Price,
                    ProductType = productVariant.ProductType.Name,
                    ProductTypeId = productVariant.ProductTypeId,
                    Qty = item.Qty
                };

                result.Data.Add(cartProduct);
            }

            return result;
        }
    }
}
