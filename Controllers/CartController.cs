using Microsoft.AspNetCore.Mvc;
using WebApp2.Models;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WebApp2.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "Cart";

        // Статический словарь для хранения товаров в корзине для каждого пользователя на основе их токена JWT
        private static readonly Dictionary<string, List<CartItem>> UserCarts = new Dictionary<string, List<CartItem>>();

        public async Task<IActionResult> Index()
        {
            var userId = GetUserIdFromToken(); // Метод для извлечения ID пользователя из токена JWT
            if (userId == null)
            {
                return Json(new { success = false, message = "Пользователь не авторизован." });
            }
            var cartItems = GetCart(userId);
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = GetUserIdFromToken(); // Method to extract user ID from JWT token
            if (userId == null)
            {
                return Json(new { success = false, message = "Пользователь не авторизован." });
            }

            var cartItems = GetCart(userId);
            var existingItem = cartItems.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity; // Increase quantity if item already in cart
            }
            else
            {
                cartItems.Add(new CartItem { ProductId = productId, Quantity = quantity }); // Add new item
            }

            SaveCart(userId, cartItems);

            // Return JSON response
            return Json(new { success = true, message = "Товар добавлен в корзину!" });
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            var userId = GetUserIdFromToken(); // Метод для извлечения ID пользователя из токена JWT
            if (userId == null)
            {
                return Json(new { success = false, message = "Пользователь не авторизован." });
            }

            var cartItems = GetCart(userId);
            // Логика для оформления заказа
            // Например, сохранение заказа в базе данных

            // Очищаем корзину после оформления заказа
            UserCarts.Remove(userId);

            return RedirectToAction("OrderConfirmation");
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }

        private List<CartItem> GetCart(string userId)
        {
            if (UserCarts.TryGetValue(userId, out var cartItems))
            {
                return cartItems;
            }
            return new List<CartItem>();
        }

        private void SaveCart(string userId, List<CartItem> cartItems)
        {
            UserCarts[userId] = cartItems;
        }

        private string GetUserIdFromToken()
        { 
                // Извлекаем ID пользователя из токена JWT
                var userIdClaim = HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "id");
                return userIdClaim?.Value;
        }
    }
}
