using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp2.Data;
using WebApp2.Models;

namespace WebApp2.Controllers
{
    public class BrosShopProductAttributesController : Controller
    {
        private readonly ApplicationContext _context;

        public BrosShopProductAttributesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: BrosShopProductAttributes
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.BrosShopProductAttributes.Include(b => b.BrosShopProduct);
            return View(await applicationContext.ToListAsync());
        }

        // GET: BrosShopProductAttributes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopProductAttribute = await _context.BrosShopProductAttributes
                .Include(b => b.BrosShopProduct)
                .FirstOrDefaultAsync(m => m.BrosShopAttributesId == id);
            if (brosShopProductAttribute == null)
            {
                return NotFound();
            }

            return View(brosShopProductAttribute);
        }

        // GET: BrosShopProductAttributes/Create
        public IActionResult Create()
        {
            ViewData["BrosShopProductId"] = new SelectList(_context.BrosShopProducts, "BrosShopProductId", "BrosShopProductId");
            return View();
        }

        // POST: BrosShopProductAttributes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BrosShopAttributesId,BrosShopProductId,BrosShopCount,BrosShopColor,BrosShopSize")] BrosShopProductAttribute brosShopProductAttribute)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brosShopProductAttribute);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrosShopProductId"] = new SelectList(_context.BrosShopProducts, "BrosShopProductId", "BrosShopProductId", brosShopProductAttribute.BrosShopProductId);
            return View(brosShopProductAttribute);
        }

        // GET: BrosShopProductAttributes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopProductAttribute = await _context.BrosShopProductAttributes.FindAsync(id);
            if (brosShopProductAttribute == null)
            {
                return NotFound();
            }
            ViewData["BrosShopProductId"] = new SelectList(_context.BrosShopProducts, "BrosShopProductId", "BrosShopProductId", brosShopProductAttribute.BrosShopProductId);
            return View(brosShopProductAttribute);
        }

        // POST: BrosShopProductAttributes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BrosShopAttributesId,BrosShopProductId,BrosShopCount,BrosShopColor,BrosShopSize")] BrosShopProductAttribute brosShopProductAttribute)
        {
            if (id != brosShopProductAttribute.BrosShopAttributesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brosShopProductAttribute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrosShopProductAttributeExists(brosShopProductAttribute.BrosShopAttributesId))
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
            ViewData["BrosShopProductId"] = new SelectList(_context.BrosShopProducts, "BrosShopProductId", "BrosShopProductId", brosShopProductAttribute.BrosShopProductId);
            return View(brosShopProductAttribute);
        }

        // GET: BrosShopProductAttributes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopProductAttribute = await _context.BrosShopProductAttributes
                .Include(b => b.BrosShopProduct)
                .FirstOrDefaultAsync(m => m.BrosShopAttributesId == id);
            if (brosShopProductAttribute == null)
            {
                return NotFound();
            }

            return View(brosShopProductAttribute);
        }

        // POST: BrosShopProductAttributes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brosShopProductAttribute = await _context.BrosShopProductAttributes.FindAsync(id);
            if (brosShopProductAttribute != null)
            {
                _context.BrosShopProductAttributes.Remove(brosShopProductAttribute);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrosShopProductAttributeExists(int id)
        {
            return _context.BrosShopProductAttributes.Any(e => e.BrosShopAttributesId == id);
        }
    }
}
