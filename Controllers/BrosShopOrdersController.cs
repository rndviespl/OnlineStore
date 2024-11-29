//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.Json;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using WebApp2.Data;
//using WebApp2.Models;

//namespace WebApp2.Controllers
//{
//    public class BrosShopOrdersController : Controller
//    {
//        private readonly ApplicationContext _context;

//        public BrosShopOrdersController(ApplicationContext context)
//        {
//            _context = context;
//        }

//        public async Task<IActionResult> Index(string sortOrder, string searchString)
//        {
//            // Получаем заказы из базы данных
//            var applicationContext = _context.BrosShopOrders.Include(b => b.BrosShopUser).AsQueryable();

//            // Применяем фильтрацию по строке поиска, если она указана
//            if (!string.IsNullOrEmpty(searchString))
//            {
//                applicationContext = applicationContext.Where(o => o.BrosShopUser.BrosShopUsername.Contains(searchString)); // Замените UserName на нужное свойство
//            }

//            // Применяем сортировку, если это необходимо
//            switch (sortOrder)
//            {
//                case "date_desc":
//                    applicationContext = applicationContext.OrderByDescending(o => o.BrosShopDateTimeOrder);
//                    break;
//                case "date_asc":
//                    applicationContext = applicationContext.OrderBy(o => o.BrosShopDateTimeOrder);
//                    break;
//                // Добавьте другие условия сортировки по мере необходимости
//                default:
//                    applicationContext = applicationContext.OrderBy(o => o.BrosShopOrderId);
//                    break;
//            }

//            // Возвращаем представление с заказами
//            var orders = await applicationContext.ToListAsync();
//            return View(orders);
//        }


//        // GET: BrosShopOrders/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var brosShopOrder = await _context.BrosShopOrders
//                .Include(b => b.BrosShopUser)
//                .FirstOrDefaultAsync(m => m.BrosShopOrderId == id);
//            if (brosShopOrder == null)
//            {
//                return NotFound();
//            }

//            return View(brosShopOrder);
//        }


//        // POST: BrosShopOrders/Checkout
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Checkout(List<CartItem> cartItems)
//        {
//            if (cartItems == null || !cartItems.Any())
//            {
//                return RedirectToAction(nameof(Index)); // Перенаправление, если корзина пуста
//            }

//            // Создание нового заказа
//            var order = new BrosShopOrder
//            {
//                BrosShopUserId = 1/* Получите ID текущего пользователя */,
//                BrosShopDateTimeOrder = DateTime.Now,
//                BrosShopOrderCompositions = new List<BrosShopOrderComposition>()
//            };
//            string orderType = "веб-сайт"; // Установите значение по умолчанию
//            order.BrosShopTypeOrder = orderType;

//            // Добавление каждого товара из корзины в состав заказа
//            foreach (var item in cartItems)
//            {
//                var product = await _context.BrosShopProducts.FindAsync(item.ProductId);
//                if (product != null)
//                {
//                    var orderComposition = new BrosShopOrderComposition
//                    {
//                        BrosShopProductId = product.BrosShopProductId,
//                        BrosShopOrderId = order.BrosShopOrderId, // Это будет установлено после сохранения заказа
//                        BrosShopQuantity = (sbyte)item.Quantity,
//                        BrosShopCost = product.BrosShopPrice * item.Quantity // Рассчитываем стоимость
//                    };

//                    order.BrosShopOrderCompositions.Add(orderComposition);
//                }
//            }

//            // Добавление заказа в контекст и сохранение
//            _context.BrosShopOrders.Add(order);
//            await _context.SaveChangesAsync();

//            return RedirectToAction(nameof(Index)); // Перенаправление на список заказов или страницу подтверждения
//        }

//        private decimal GetProductPrice(int productId)
//        {
//            try
//            {
//                // Ищем продукт по его ID в базе данных
//                var product = _context.BrosShopProducts.FirstOrDefault(i => i.BrosShopProductId == productId);
//                // Проверяем, найден ли продукт
//                if (product == null)
//                {
//                    return 0; // Возвращаем 0, если продукт не найден
//                }

//                // Возвращаем стоимость продукта
//                return product.BrosShopPrice;
//            }
//            catch (Exception ex)
//            {
//                // Возвращаем 0 в случае ошибки
//                return 0;
//            }
//        }



//        // GET: BrosShopOrders/Create
//        public IActionResult Create()
//        {
//            ViewData["BrosShopUserId"] = new SelectList(_context.BrosShopUsers, "BrosShopUserId", "BrosShopUserId");
//            return View();
//        }

//        // POST: BrosShopOrders/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("BrosShopOrderId,BrosShopUserId,BrosShopDateTimeOrder,BrosShopTypeOrder")] BrosShopOrder brosShopOrder)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Add(brosShopOrder);
//                await _context.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }
//            ViewData["BrosShopUserId"] = new SelectList(_context.BrosShopUsers, "BrosShopUserId", "BrosShopUserId", brosShopOrder.BrosShopUserId);
//            return View(brosShopOrder);
//        }

//        // GET: BrosShopOrders/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var brosShopOrder = await _context.BrosShopOrders.FindAsync(id);
//            if (brosShopOrder == null)
//            {
//                return NotFound();
//            }
//            ViewData["BrosShopUserId"] = new SelectList(_context.BrosShopUsers, "BrosShopUserId", "BrosShopUserId", brosShopOrder.BrosShopUserId);
//            return View(brosShopOrder);
//        }

//        // POST: BrosShopOrders/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("BrosShopOrderId,BrosShopUserId,BrosShopDateTimeOrder,BrosShopTypeOrder")] BrosShopOrder brosShopOrder)
//        {
//            if (id != brosShopOrder.BrosShopOrderId)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    _context.Update(brosShopOrder);
//                    await _context.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!BrosShopOrderExists(brosShopOrder.BrosShopOrderId))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            ViewData["BrosShopUserId"] = new SelectList(_context.BrosShopUsers, "BrosShopUserId", "BrosShopUserId", brosShopOrder.BrosShopUserId);
//            return View(brosShopOrder);
//        }

//        // GET: BrosShopOrders/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var brosShopOrder = await _context.BrosShopOrders
//                .Include(b => b.BrosShopUser)
//                .FirstOrDefaultAsync(m => m.BrosShopOrderId == id);
//            if (brosShopOrder == null)
//            {
//                return NotFound();
//            }

//            return View(brosShopOrder);
//        }

//        // POST: BrosShopOrders/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var brosShopOrder = await _context.BrosShopOrders.FindAsync(id);
//            if (brosShopOrder != null)
//            {
//                _context.BrosShopOrders.Remove(brosShopOrder);
//            }

//            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }


//        private bool BrosShopOrderExists(int id)
//        {
//            return _context.BrosShopOrders.Any(e => e.BrosShopOrderId == id);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult RemoveFromCart(int productId)
//        {
//            // Получаем текущую корзину из куки
//            var cart = HttpContext.Request.Cookies["Cart"];
//            var cartItems = string.IsNullOrEmpty(cart) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(cart);

//            // Удаляем товар из корзины
//            var itemToRemove = cartItems.FirstOrDefault(item => item.ProductId == productId);
//            if (itemToRemove != null)
//            {
//                cartItems.Remove(itemToRemove);
//            }

//            // Сохраняем обновленную корзину обратно в куки
//            var updatedCart = JsonSerializer.Serialize(cartItems);
//            HttpContext.Response.Cookies.Append("Cart", updatedCart, new CookieOptions { HttpOnly = true });

//            // Перенаправляем на страницу с корзиной
//            return RedirectToAction("Index"); // Убедитесь, что у вас есть метод Cart, который отображает корзину
//        }

//        public IActionResult Cart()
//        {
//            var cart = HttpContext.Request.Cookies["Cart"];
//            var cartItems = string.IsNullOrEmpty(cart) ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(cart);
//            return View(cartItems); // Возвращаем представление с корзиной
//        }
//    }
//}
