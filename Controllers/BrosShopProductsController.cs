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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BrosShopProductsController(ApplicationContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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

        public class AddToCartResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, int sizeId)
        {
            if (quantity <= 0)
            {
                return Json(new AddToCartResponse { Success = false, Message = "Количество должно быть больше нуля." });
            }

            // Проверяем, существует ли продукт
            var productExists = _context.BrosShopProducts.Any(p => p.BrosShopProductId == productId);
            if (!productExists)
            {
                return Json(new AddToCartResponse { Success = false, Message = "Товар не найден." });
            }

            var cartItems = GetCartFromCookies();
            var existingItem = cartItems.FirstOrDefault(i => i.ProductId == productId && i.SizeId == sizeId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity; // Увеличиваем количество, если товар уже в корзине
            }
            else
            {
                cartItems.Add(new CartItem { ProductId = productId, Quantity = quantity, SizeId = sizeId }); // Добавляем новый товар
            }

            SaveCartToCookies(cartItems);

            return Json(new AddToCartResponse { Success = true, Message = "Товар добавлен в корзину!" });
        }




        protected List<CartItem> GetCartFromCookies()
        {
            if (Request.Cookies.TryGetValue(CartCookieKey, out var cookieValue))
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(cookieValue) ?? new List<CartItem>();
            }
            return new List<CartItem>(); // Возвращаем пустой список, если куки отсутствуют
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

    public class TestableBrosShopProductsController : BrosShopProductsController
    {
        public TestableBrosShopProductsController(ApplicationContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        public List<CartItem> GetCartItemsForTesting()
        {
            return GetCartFromCookies();
        }
    }


}
