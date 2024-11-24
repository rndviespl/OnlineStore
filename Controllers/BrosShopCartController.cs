using Microsoft.AspNetCore.Mvc;
using WebApp2.Models;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApp2.Data;

namespace WebApp2.Controllers
{
    public class BrosShopCartController : Controller
    {
        private const string CartCookieKey = "Cart";
        private readonly ApplicationContext _context;
        // Конструктор, принимающий ApplicationContext
        public BrosShopCartController(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var cartItems = GetCartFromCookies();
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Количество должно быть больше нуля." });
            }

            var cartItems = GetCartFromCookies();
            var existingItem = cartItems.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity; // Увеличиваем количество, если товар уже в корзине
            }
            else
            {
                cartItems.Add(new CartItem { ProductId = productId, Quantity = quantity }); // Добавляем новый товар
            }

            SaveCartToCookies(cartItems);

            // Возвращаем JSON-ответ
            return Json(new { success = true, message = "Товар добавлен в корзину!" });
        }


        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var cartItems = GetCartFromCookies();
            if (!cartItems.Any())
            {
                return Json(new { success = false, message = "Корзина пуста." });
            }

            var brosShopOrder = new BrosShopOrder
            {
                BrosShopUserId = 1, // Замените на фактический ID пользователя
                BrosShopDateTimeOrder = DateTime.UtcNow,
                BrosShopTypeOrder = "веб-сайт", // Убедитесь, что это допустимое значение
                BrosShopOrderCompositions = new List<BrosShopOrderComposition>()
            };

            foreach (var item in cartItems)
            {
                var product = await _context.BrosShopProducts.FindAsync(item.ProductId);
                if (product != null)
                {
                    brosShopOrder.BrosShopOrderCompositions.Add(new BrosShopOrderComposition
                    {
                        BrosShopProductId = product.BrosShopProductId,
                        BrosShopQuantity = (sbyte)item.Quantity,
                        BrosShopCost = product.BrosShopPrice * item.Quantity // Рассчитываем стоимость
                    });
                }
                else
                {
                    // Логирование или обработка случая, когда продукт не найден
                    return Json(new { success = false, message = $"Продукт с ID {item.ProductId} не найден." });
                }
            }

            // Сохраняем заказ в базе данных
            _context.BrosShopOrders.Add(brosShopOrder);
            await _context.SaveChangesAsync();

            // Очищаем корзину после оформления заказа
            Response.Cookies.Delete(CartCookieKey);

            return Json(new { success = true, message = "Заказ успешно оформлен!" });
        }


        public IActionResult OrderConfirmation()
        {
            return View();
        }

        private List<CartItem> GetCartFromCookies()
        {
            if (Request.Cookies.TryGetValue(CartCookieKey, out var cookieValue))
            {
                var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cookieValue) ?? new List<CartItem>();
                // Выводим содержимое корзины для отладки
                Console.WriteLine("Товары в корзине: " + JsonConvert.SerializeObject(cartItems));
                return cartItems;
            }
            return new List<CartItem>();
        }

        private void SaveCartToCookies(List<CartItem> cartItems)
        {
            var cookieValue = JsonConvert.SerializeObject(cartItems);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(3), // Устанавливаем срок действия куки
                HttpOnly = true // Запрещаем доступ к куки через JavaScript
            };
            Response.Cookies.Append(CartCookieKey, cookieValue, cookieOptions);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cartItems = GetCartFromCookies();
            var itemToRemove = cartItems.FirstOrDefault(i => i.ProductId == productId);

            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove); // Удаляем товар из корзины
                SaveCartToCookies(cartItems); // Сохраняем обновленную корзину в куки
            }

            return Json(new { success = true, message = "Товар удален из корзины." });
        }
    }
}
