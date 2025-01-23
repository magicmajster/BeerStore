using BeerStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace BeerStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ShoppingCartService _shoppingCartService;

        public ShoppingCartController(ShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            var cart = _shoppingCartService.GetCartItems();
            ViewBag.TotalPrice = _shoppingCartService.GetTotalPrice();
            return View(cart);
        }

        public IActionResult AddToCart(int id)
        {
            _shoppingCartService.AddToCart(id);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            _shoppingCartService.RemoveFromCart(id);
            return RedirectToAction("Index");
        }

        public IActionResult ClearCart()
        {
            _shoppingCartService.ClearCart();
            return RedirectToAction("Index");
        }
    }
}
