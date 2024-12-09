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
            var products = await _context.BrosShopProducts
                .Where(p => cartItems.Select(ci => ci.ProductId).Contains(p.BrosShopProductId))
                .ToListAsync();

            var viewModel = new CartViewModel
            {
                CartItems = cartItems,
                Products = products // Добавляем список продуктов
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            if (quantity < 1 || quantity > 254)
            {
                return Json(new { success = false, message = "Количество должно быть от 1 до 254." });
            }

            var cartItems = GetCartFromCookies();
            var existingItem = cartItems.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                // Проверяем, не превышает ли новое количество максимальное значение
                if (quantity > 254)
                {
                    return Json(new { success = false, message = "Максимальное количество для этого товара - 254." });
                }
                existingItem.Quantity = quantity; // Обновляем количество
            }
            else
            {
                cartItems.Add(new CartItem { ProductId = productId, Quantity = quantity }); // Добавляем новый товар, если его нет
            }

            SaveCartToCookies(cartItems);

            return Json(new { success = true, message = "Корзина обновлена!" });
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            // Получаем текущую корзину из куки
            var cartItems = GetCartFromCookies();
            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId);

            // Проверяем общее количество товара в корзине
            int currentQuantity = existingItem != null ? existingItem.Quantity : 0;
            int totalQuantity = currentQuantity + quantity;

            if (totalQuantity > 254)
            {
                return Json(new { success = false, message = "Вы не можете добавить более 254 единиц этого товара." });
            }

            // Логика добавления товара в корзину
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cartItems.Add(new CartItem { ProductId = productId, Quantity = quantity });
            }

            // Сохранение обновленной корзины в куки
            SaveCartToCookies(cartItems);

            return Json(new { success = true, message = "Товар добавлен в корзину." });
        }


        [HttpGet]
        public IActionResult GetCartQuantity(int productId)
        {
            var cartItems = GetCartFromCookies();
            var existingItem = cartItems.FirstOrDefault(item => item.ProductId == productId);
            int currentQuantity = existingItem != null ? existingItem.Quantity : 0;

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
            var claimsIdentity = principal.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                foreach (var claim in claimsIdentity.Claims)
                {
                    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }
            }
            // Получаем username из токена
            var usernameClaim = principal.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (usernameClaim == null)
            {
                return Json(new { success = false, message = "Username не найден в токене." });
            }
            var username = usernameClaim.Value;

            // Находим пользователя по username
            var user = await _context.BrosShopUsers.FirstOrDefaultAsync(u => u.BrosShopUsername == username);
            if (user == null)
            {
                return Json(new { success = false, message = "Пользователь не найден." });
            }

            var brosShopOrder = new BrosShopOrder
            {
                BrosShopUserId = user.BrosShopUserId, // Замените на фактический ID пользователя
                BrosShopDateTimeOrder = DateTime.UtcNow,
                BrosShopTypeOrder = "веб-сайт",
                BrosShopOrderCompositions = new List<BrosShopOrderComposition>()
            };
            var orderDetails = new List<OrderDetail>(); // Создаем список для деталей заказа

            foreach (var item in cartItems)
            {
                var product = await _context.BrosShopProducts.FindAsync(item.ProductId);
                if (product != null)
                {
                    brosShopOrder.BrosShopOrderCompositions.Add(new BrosShopOrderComposition
                    {
                        BrosShopProductId = product.BrosShopProductId,
                        BrosShopQuantity = (sbyte)item.Quantity,
                        BrosShopCost = product.BrosShopPrice * item.Quantity
                    });

                    // Добавляем детали заказа в список
                    orderDetails.Add(new OrderDetail
                    {
                        ProductTitle = product.BrosShopTitle,
                        Quantity = item.Quantity,
                        UnitPrice = product.BrosShopPrice,
                        TotalPrice = product.BrosShopPrice * item.Quantity
                    });
                }
                else
                {
                    return Json(new { success = false, message = $"Продукт с ID {item.ProductId} не найден." });
                }
            }

            _context.BrosShopOrders.Add(brosShopOrder);
            await _context.SaveChangesAsync();

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

        private List<OrderDetail> GetOrderDetails()
        {
            // Здесь вы можете реализовать логику для получения деталей заказа
            return new List<OrderDetail>(); // Возвращаем пустой список или заполненный
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
            // Получаем текущую корзину из куки
            var cart = HttpContext.Request.Cookies[CartCookieKey];
            var cartItems = string.IsNullOrEmpty(cart) ? new List<CartItem>() : JsonConvert.DeserializeObject<List<CartItem>>(cart);

            // Удаляем товар из корзины
            var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);
            if (itemToRemove != null)
            {
                cartItems.Remove(itemToRemove);
            }

            // Сохраняем обновленную корзину обратно в куки
            var updatedCart = JsonConvert.SerializeObject(cartItems);
            HttpContext.Response.Cookies.Append(CartCookieKey, updatedCart, new CookieOptions { HttpOnly = true });

            // Перенаправляем на страницу корзины
            return RedirectToAction("Index"); // Перенаправляем на метод Cart, который отображает корзину
        }
    }
}