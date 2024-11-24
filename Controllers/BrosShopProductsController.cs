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
            .Include(b => b.BrosShopCategory)
            .Include(p => p.BrosShopImages) // Загружаем все изображения
            .FirstOrDefaultAsync(m => m.BrosShopProductId == id);

            if (brosShopProduct == null)
            {
                return NotFound();
            }

            return View(brosShopProduct);
        }

        // GET: BrosShopProducts/Create
        public IActionResult Create()
        {
            ViewData["BrosShopCategoryId"] = new SelectList(_context.BrosShopCategories, "BrosShopCategoryId", "BrosShopCategoryId");
            return View();
        }

        // POST: BrosShopProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BrosShopProductId,BrosShopPrice,BrosShopTitle,BrosShopDiscountPercent,BrosShopWbarticul,BrosShopDescription,BrosShopCategoryId,BrosShopPurcharesePrice")] BrosShopProduct brosShopProduct)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brosShopProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrosShopCategoryId"] = new SelectList(_context.BrosShopCategories, "BrosShopCategoryId", "BrosShopCategoryId", brosShopProduct.BrosShopCategoryId);
            return View(brosShopProduct);
        }

        // GET: BrosShopProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopProduct = await _context.BrosShopProducts.FindAsync(id);
            if (brosShopProduct == null)
            {
                return NotFound();
            }
            ViewData["BrosShopCategoryId"] = new SelectList(_context.BrosShopCategories, "BrosShopCategoryId", "BrosShopCategoryId", brosShopProduct.BrosShopCategoryId);
            return View(brosShopProduct);
        }

        // POST: BrosShopProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BrosShopProductId,BrosShopPrice,BrosShopTitle,BrosShopDiscountPercent,BrosShopWbarticul,BrosShopDescription,BrosShopCategoryId,BrosShopPurcharesePrice")] BrosShopProduct brosShopProduct)
        {
            if (id != brosShopProduct.BrosShopProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brosShopProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrosShopProductExists(brosShopProduct.BrosShopProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrosShopCategoryId"] = new SelectList(_context.BrosShopCategories, "BrosShopCategoryId", "BrosShopCategoryId", brosShopProduct.BrosShopCategoryId);
            return View(brosShopProduct);
        }

        // GET: BrosShopProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopProduct = await _context.BrosShopProducts
                .Include(b => b.BrosShopCategory)
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
