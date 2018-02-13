using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polly.CircuitBreaker;
using ShoesOnContainers.Web.WebMvc.Models;
using ShoesOnContainers.Web.WebMvc.Models.CartModels;
using ShoesOnContainers.Web.WebMvc.Services;

namespace ShoesOnContainers.Web.WebMvc.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ICatalogService _catalog;
        private readonly IIdentityService<ApplicationUser> _identityService;

        public CartController(
            ICartService cartService,
            ICatalogService catalog,
            IIdentityService<ApplicationUser> identityService)
        {
            _cartService = cartService;
            _catalog = catalog;
            _identityService = identityService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Dictionary<string, int> quantities, string action)
        {
            try
            {
                var user = _identityService.Get(HttpContext.User);
                var basket = await _cartService.SetQuantities(user, quantities);
                var vm = await _cartService.UpdateCart(basket);
            }
            catch (BrokenCircuitException)
            {
                HandleBrokenCircuitException();
            }

            return RedirectToAction("Index", "Catalog");
        }


        public async Task<IActionResult> AddToCart(CatalogItem productDetails)
        {
            try
            {
                if (productDetails.Id != null)
                {
                    var user = _identityService.Get(HttpContext.User);
                    var product = new CartItem
                    {
                        Id = Guid.NewGuid().ToString(),
                        Quantity = 1,
                        ProductName = productDetails.Name,
                        PictureUrl = productDetails.PictureUrl,
                        UnitPrice = productDetails.Price,
                        ProductId = productDetails.Id
                    };

                    await _cartService.AddItemToCart(user, product);
                }

                return RedirectToAction("Index", "Catalog");
            }
            catch (BrokenCircuitException)
            {
                HandleBrokenCircuitException();
            }

            return RedirectToAction("Index", "Catalog");
        }

        private void HandleBrokenCircuitException()
        {
            TempData["BasketInoperativeMsg"] = "cart Service inoperative, please try later on.";
        }
    }
}