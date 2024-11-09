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
    public class BrosShopImagesController : Controller
    {
        private readonly ApplicationContext _context;

        public BrosShopImagesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: BrosShopImages
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.BrosShopImages.Include(b => b.BrosShopProduct);
            return View(await applicationContext.ToListAsync());
        }

        // GET: BrosShopImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopImage = await _context.BrosShopImages
                .Include(b => b.BrosShopProduct)
                .FirstOrDefaultAsync(m => m.BrosShopImagesId == id);
            if (brosShopImage == null)
            {
                return NotFound();
            }

            return View(brosShopImage);
        }

        // GET: BrosShopImages/Create
        public IActionResult Create()
        {
            ViewData["BrosShopProductId"] = new SelectList(_context.BrosShopProducts, "BrosShopProductId", "BrosShopProductId");
            return View();
        }

        // POST: BrosShopImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BrosShopImagesId,BrosShopProductId,BrosShopImageTitle")] BrosShopImage brosShopImage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brosShopImage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrosShopProductId"] = new SelectList(_context.BrosShopProducts, "BrosShopProductId", "BrosShopProductId", brosShopImage.BrosShopProductId);
            return View(brosShopImage);
        }

        // GET: BrosShopImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopImage = await _context.BrosShopImages.FindAsync(id);
            if (brosShopImage == null)
            {
                return NotFound();
            }
            ViewData["BrosShopProductId"] = new SelectList(_context.BrosShopProducts, "BrosShopProductId", "BrosShopProductId", brosShopImage.BrosShopProductId);
            return View(brosShopImage);
        }

        // POST: BrosShopImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BrosShopImagesId,BrosShopProductId,BrosShopImageTitle")] BrosShopImage brosShopImage)
        {
            if (id != brosShopImage.BrosShopImagesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brosShopImage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrosShopImageExists(brosShopImage.BrosShopImagesId))
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
            ViewData["BrosShopProductId"] = new SelectList(_context.BrosShopProducts, "BrosShopProductId", "BrosShopProductId", brosShopImage.BrosShopProductId);
            return View(brosShopImage);
        }

        // GET: BrosShopImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopImage = await _context.BrosShopImages
                .Include(b => b.BrosShopProduct)
                .FirstOrDefaultAsync(m => m.BrosShopImagesId == id);
            if (brosShopImage == null)
            {
                return NotFound();
            }

            return View(brosShopImage);
        }

        // POST: BrosShopImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brosShopImage = await _context.BrosShopImages.FindAsync(id);
            if (brosShopImage != null)
            {
                _context.BrosShopImages.Remove(brosShopImage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrosShopImageExists(int id)
        {
            return _context.BrosShopImages.Any(e => e.BrosShopImagesId == id);
        }
    }
}
