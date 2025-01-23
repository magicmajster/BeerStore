using BeerStore.Data;
using BeerStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BeerStore.Services
{
    public class ShoppingCartService
    {
        private const string CartSessionKey = "ShoppingCart";

        private readonly ApplicationDbContext _context;
        private readonly ISession _session;

        public ShoppingCartService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _session = httpContextAccessor.HttpContext?.Session
                       ?? throw new InvalidOperationException("Session is not available.");
        }

        public List<ShoppingCartItem> GetCartItems()
        {
            var cartJson = _session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<ShoppingCartItem>();
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                    PropertyNameCaseInsensitive = true // Ensure case-insensitive deserialization
                };

                return JsonSerializer.Deserialize<List<ShoppingCartItem>>(cartJson, options);
            }
            catch (JsonException ex)
            {
                // Log the exception or handle it
                Console.WriteLine($"Deserialization error: {ex.Message}");
                // Clear corrupted session data
                ClearCart();
                return new List<ShoppingCartItem>();
            }
        }

        private void SaveCart(List<ShoppingCartItem> cart)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                WriteIndented = true
            };

            var cartJson = JsonSerializer.Serialize(cart, options);
            _session.SetString(CartSessionKey, cartJson);
        }


        public void AddToCart(int beerId, int quantity = 1)
        {
            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(c => c.BeerId == beerId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                var beer = _context.Beers.Include(b => b.Country).FirstOrDefault(b => b.Id == beerId);
                if (beer != null)
                {
                    cart.Add(new ShoppingCartItem { BeerId = beerId, Beer = beer, Quantity = quantity });
                }
            }

            SaveCart(cart);
        }

        public void ClearCart()
        {
            _session.Remove(CartSessionKey); // Clear the session key
        }


        public void RemoveFromCart(int beerId)
        {
            var cart = GetCartItems();
            var cartItem = cart.FirstOrDefault(c => c.BeerId == beerId);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCart(cart);
            }
        }

        public decimal GetTotalPrice()
        {
            var cart = GetCartItems();
            return cart.Sum(c => c.Beer.Price * c.Quantity);
        }


    }
}
