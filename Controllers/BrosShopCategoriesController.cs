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
    public class BrosShopCategoriesController : Controller
    {
        private readonly ApplicationContext _context;

        public BrosShopCategoriesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: BrosShopCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.BrosShopCategories.ToListAsync());
        }

        // GET: BrosShopCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopCategory = await _context.BrosShopCategories
                .FirstOrDefaultAsync(m => m.BrosShopCategoryId == id);
            if (brosShopCategory == null)
            {
                return NotFound();
            }

            return View(brosShopCategory);
        }

        // GET: BrosShopCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BrosShopCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BrosShopCategoryId,BrosShopCategoryTitle")] BrosShopCategory brosShopCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brosShopCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brosShopCategory);
        }

        // GET: BrosShopCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopCategory = await _context.BrosShopCategories.FindAsync(id);
            if (brosShopCategory == null)
            {
                return NotFound();
            }
            return View(brosShopCategory);
        }

        // POST: BrosShopCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BrosShopCategoryId,BrosShopCategoryTitle")] BrosShopCategory brosShopCategory)
        {
            if (id != brosShopCategory.BrosShopCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brosShopCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrosShopCategoryExists(brosShopCategory.BrosShopCategoryId))
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
            return View(brosShopCategory);
        }

        // GET: BrosShopCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopCategory = await _context.BrosShopCategories
                .FirstOrDefaultAsync(m => m.BrosShopCategoryId == id);
            if (brosShopCategory == null)
            {
                return NotFound();
            }

            return View(brosShopCategory);
        }

        // POST: BrosShopCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brosShopCategory = await _context.BrosShopCategories.FindAsync(id);
            if (brosShopCategory != null)
            {
                _context.BrosShopCategories.Remove(brosShopCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrosShopCategoryExists(int id)
        {
            return _context.BrosShopCategories.Any(e => e.BrosShopCategoryId == id);
        }
    }
}
