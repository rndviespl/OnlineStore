using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApp2.Data;
using WebApp2.Models;

namespace WebApp2.Controllers
{
    public class BrosShopProductsController : Controller
    {
        private readonly ApplicationContext _context;
        private const string CartCookieKey = "Cart";

        public BrosShopProductsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: BrosShopProducts
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.BrosShopProducts.Include(b => b.BrosShopCategory);
            return View(await applicationContext.ToListAsync());
        }

        // GET: BrosShopProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopProduct = await _context.BrosShopProducts
                .Include(b => b.BrosShopCategory) // Загружаем категорию
                .Include(p => p.BrosShopImages) // Загружаем все изображения
                .Include(p => p.BrosShopProductAttributes) // Загружаем атрибуты продукта
                    .ThenInclude(pa => pa.BrosShopSizeNavigation) // Загружаем размеры
                .FirstOrDefaultAsync(m => m.BrosShopProductId == id);

            if (brosShopProduct == null)
            {
                return NotFound();
            }

            return View(brosShopProduct);
        }


        // POST: BrosShopProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brosShopProduct = await _context.BrosShopProducts.FindAsync(id);
            if (brosShopProduct != null)
            {
                _context.BrosShopProducts.Remove(brosShopProduct);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrosShopProductExists(int id)
        {
            return _context.BrosShopProducts.Any(e => e.BrosShopProductId == id);
        }


        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, int sizeId) // Change size parameter to sizeId
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Количество должно быть больше нуля." });
            }

            var cartItems = GetCartFromCookies();
            var existingItem = cartItems.FirstOrDefault(i => i.ProductId == productId && i.SizeId == sizeId); // Check for existing item with the same sizeId

            if (existingItem != null)
            {
                existingItem.Quantity += quantity; // Increase quantity if item already in cart
            }
            else
            {
                cartItems.Add(new CartItem { ProductId = productId, Quantity = quantity, SizeId = sizeId }); // Add new item with sizeId
            }

            SaveCartToCookies(cartItems);

            // Return JSON response
            return Json(new { success = true, message = "Товар добавлен в корзину!" });
        }


        private List<CartItem> GetCartFromCookies()
        {
            if (Request.Cookies.TryGetValue(CartCookieKey, out var cookieValue))
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(cookieValue) ?? new List<CartItem>();
            }
            return new List<CartItem>();
        }

        private void SaveCartToCookies(List<CartItem> cartItems)
        {
            var cookieValue = JsonConvert.SerializeObject(cartItems);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30), // Устанавливаем срок действия куки
                HttpOnly = true // Запрещаем доступ к куки через JavaScript
            };
            Response.Cookies.Append(CartCookieKey, cookieValue, cookieOptions);
        }
    }
}
