using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApp2.Data;
using WebApp2.Models;

namespace WebApp2.Controllers
{
    public partial class BrosShopCartController : Controller
    {
        private const string CartCookieKey = "Cart";
        private readonly ApplicationContext _context;
        private readonly IConfiguration _configuration;

        public BrosShopCartController(ApplicationContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var cartItems = GetCartFromCookies();
            var productIds = cartItems.Select(ci => ci.ProductId).ToList();

            var products = await _context.BrosShopProducts
                .Include(p => p.BrosShopProductAttributes) // Загружаем атрибуты продукта
                    .ThenInclude(a => a.BrosShopSizeNavigation) // Загружаем размеры
                .Where(p => productIds.Contains(p.BrosShopProductId))
                .ToListAsync();

            var viewModel = new CartViewModel
            {
                CartItems = cartItems,
                Products = products
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity, int sizeId)
        {
            if (quantity < 1 || quantity > 100)
            {
                return Json(new { success = false, message = "Количество должно быть от 1 до 100." });
            }

            var cartItems = GetCartFromCookies();
            var existingItem = cartItems.FirstOrDefault(i => i.ProductId == productId && i.SizeId == sizeId);

            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
            }
            else
            {
                cartItems.Add(new CartItem { ProductId = productId, Quantity = quantity, SizeId = sizeId });
            }

            SaveCartToCookies(cartItems);
            return Json(new { success = true, message = "Корзина обновлена!" });
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity, int sizeId)
        {
            // Проверка на допустимое количество
            if (quantity < 1 || quantity > 100)
            {
                return Json(new { success = false, message = "Количество должно быть от 1 до 100." });
            }

            var cartItems = GetCartFromCookies();
            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId && item.SizeId == sizeId);

            // Если товар уже существует в корзине, увеличиваем количество
            if (existingItem != null)
            {
                int totalQuantity = existingItem.Quantity + quantity;

                // Проверка на превышение максимального количества
                if (totalQuantity > 100)
                {
                    return Json(new { success = false, message = "Вы не можете добавить более 100 единиц этого товара." });
                }

                existingItem.Quantity = totalQuantity; // Обновляем количество
            }
            else
            {
                // Добавляем новый товар в корзину
                cartItems.Add(new CartItem { ProductId = productId, Quantity = quantity, SizeId = sizeId });
            }

            SaveCartToCookies(cartItems);
            return Json(new { success = true, message = "Товар добавлен в корзину." });
        }

        [HttpGet]
        public IActionResult GetCartQuantity(int productId)
        {
            var cartItems = GetCartFromCookies();
            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
            int currentQuantity = existingItem?.Quantity ?? 0;

            return Json(new { currentQuantity });
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var cartItems = GetCartFromCookies();
            if (!cartItems.Any())
            {
                return Json(new { success = false, message = "Корзина пуста." });
            }

            var jwtToken = Request.Cookies["Token"];
            var secretKey = _configuration["ApiSettings:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var usernameClaim = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (usernameClaim == null)
            {
                return Json(new { success = false, message = "Username не найден в токене." });
            }
            var username = usernameClaim.Value;

            var user = await _context.BrosShopUsers.FirstOrDefaultAsync(u => u.BrosShopUsername == username);
            if (user == null)
            {
                return Json(new { success = false, message = "Пользователь не найден." });
            }

            // Создаем новый заказ
            var brosShopOrder = new BrosShopOrder
            {
                BrosShopUserId = user.BrosShopUserId,
                BrosShopDateTimeOrder = DateTime.UtcNow,
                BrosShopTypeOrder = "веб-сайт"
            };

            // Добавляем заказ в контекст и сохраняем изменения, чтобы получить идентификатор заказа
            _context.BrosShopOrders.Add(brosShopOrder);
            await _context.SaveChangesAsync();

            // Теперь у нас есть идентификатор заказа
            var orderId = brosShopOrder.BrosShopOrderId;

            var orderDetails = new List<OrderDetail>(); // Создаем список для деталей заказа

            foreach (var item in cartItems)
            {
                // Получаем атрибут продукта по ProductId и SizeId
                var productAttribute = await _context.BrosShopProductAttributes
                    .Include(pa => pa.BrosShopProduct) // Загружаем продукт
                    .Include(pa => pa.BrosShopSizeNavigation) // Загружаем размер
                    .FirstOrDefaultAsync(pa => pa.BrosShopAttributesId == item.ProductId && pa.BrosShopSize == item.SizeId);

                if (productAttribute != null)
                {
                    // Создаем составную часть заказа
                    var orderComposition = new BrosShopOrderComposition
                    {
                        BrosShopOrderId = orderId, // Устанавливаем идентификатор заказа
                        BrosShopAttributesId = productAttribute.BrosShopAttributesId,
                        BrosShopQuantity = (sbyte)item.Quantity,
                        BrosShopCost = productAttribute.BrosShopProduct.BrosShopPrice
                    };

                    // Добавляем составную часть заказа в контекст
                    _context.BrosShopOrderCompositions.Add(orderComposition);

                    // Добавляем детали заказа в список
                    orderDetails.Add(new OrderDetail
                    {
                        ProductTitle = productAttribute.BrosShopProduct.BrosShopTitle,
                        Quantity = item.Quantity,
                        UnitPrice = productAttribute.BrosShopProduct.BrosShopPrice,
                        TotalPrice = productAttribute.BrosShopProduct.BrosShopPrice * item.Quantity
                    });
                }
                else
                {
                    return Json(new { success = false, message = $"Продукт с ID {item.ProductId} и размером {item.SizeId} не найден." });
                }
            }

            // Сохраняем изменения для составных частей заказа
            await _context.SaveChangesAsync();

            // Удаляем корзину из куки
            Response.Cookies.Delete(CartCookieKey);

            // Перенаправляем на страницу подтверждения заказа с деталями
            return View("OrderConfirmation", orderDetails);
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
                return cartItems;
            }
            return new List<CartItem>();
        }

        private void SaveCartToCookies(List<CartItem> cartItems)
        {
            var cookieValue = JsonConvert.SerializeObject(cartItems);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(3),
                HttpOnly = true
            };
            Response.Cookies.Append(CartCookieKey, cookieValue, cookieOptions);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int productId)
        {
            var cartItems = GetCartFromCookies();
            var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
            }

            SaveCartToCookies(cartItems);
            return RedirectToAction("Index");
        }
    }
}

