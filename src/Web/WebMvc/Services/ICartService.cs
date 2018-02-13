using System.Collections.Generic;
using System.Threading.Tasks;
using ShoesOnContainers.Web.WebMvc.Models;
using ShoesOnContainers.Web.WebMvc.Models.CartModels;

namespace ShoesOnContainers.Web.WebMvc.Services
{
    public interface ICartService
    {
        Task<Cart> GetCart(ApplicationUser user);
        Task AddItemToCart(ApplicationUser user, CartItem product);
        Task<Cart> UpdateCart(Cart cart);

        Task<Cart> SetQuantities(ApplicationUser user, Dictionary<string, int> quantities);

        // Order MapCartToOrder(Cart Cart);
        Task ClearCart(ApplicationUser user);
    }
}