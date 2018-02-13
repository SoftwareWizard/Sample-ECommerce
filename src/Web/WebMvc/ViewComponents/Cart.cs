using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using ShoesOnContainers.Web.WebMvc.Models;
using ShoesOnContainers.Web.WebMvc.Services;
using ShoesOnContainers.Web.WebMvc.ViewModels;

namespace ShoesOnContainers.Web.WebMvc.ViewComponents
{
    public class Cart : ViewComponent
    {
        private readonly ICartService _cartSvc;

        public Cart(ICartService cartSvc)
        {
            _cartSvc = cartSvc;
        }

        public async Task<IViewComponentResult> InvokeAsync(ApplicationUser user)
        {
            var vm = new CartComponentViewModel();
            try
            {
                var cart = await _cartSvc.GetCart(user);
                vm.ItemsInCart = cart.Items.Count;
                vm.TotalCost = cart.Total();
                return View(vm);
            }
            catch (BrokenCircuitException)
            {
                ViewBag.IsBasketInoperative = true;
            }

            return View(vm);
        }
    }
}