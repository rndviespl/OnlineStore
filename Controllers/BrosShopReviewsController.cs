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
    public class BrosShopReviewsController : Controller
    {
        private readonly ApplicationContext _context;

        public BrosShopReviewsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: BrosShopReviews
        public async Task<IActionResult> Index()
        {
            var applicationContext = _context.BrosShopReviews.Include(b => b.BrosShopProduct).Include(b => b.BrosShopUser);
            return View(await applicationContext.ToListAsync());
        }

        // GET: BrosShopReviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopReview = await _context.BrosShopReviews
                .Include(b => b.BrosShopProduct)
                .Include(b => b.BrosShopUser)
                .FirstOrDefaultAsync(m => m.BrosShopReviewId == id);
            if (brosShopReview == null)
            {
                return NotFound();
            }

            return View(brosShopReview);
        }

        // GET: BrosShopReviews/Create
        public IActionResult Create()
        {
            ViewData["BrosShopProductId"] = new SelectList(_context.BrosShopProducts, "BrosShopProductId", "BrosShopProductId");
            ViewData["BrosShopUserId"] = new SelectList(_context.BrosShopUsers, "BrosShopUserId", "BrosShopUserId");
            return View();
        }

        // POST: BrosShopReviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BrosShopReviewId,BrosShopProductId,BrosShopUserId,BrosShopRating,BrosShopComment,BrosShopDateTime")] BrosShopReview brosShopReview)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brosShopReview);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrosShopProductId"] = new SelectList(_context.BrosShopProducts, "BrosShopProductId", "BrosShopProductId", brosShopReview.BrosShopProductId);
            ViewData["BrosShopUserId"] = new SelectList(_context.BrosShopUsers, "BrosShopUserId", "BrosShopUserId", brosShopReview.BrosShopUserId);
            return View(brosShopReview);
        }

        // GET: BrosShopReviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopReview = await _context.BrosShopReviews.FindAsync(id);
            if (brosShopReview == null)
            {
                return NotFound();
            }
            ViewData["BrosShopProductId"] = new SelectList(_context.BrosShopProducts, "BrosShopProductId", "BrosShopProductId", brosShopReview.BrosShopProductId);
            ViewData["BrosShopUserId"] = new SelectList(_context.BrosShopUsers, "BrosShopUserId", "BrosShopUserId", brosShopReview.BrosShopUserId);
            return View(brosShopReview);
        }

        // POST: BrosShopReviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BrosShopReviewId,BrosShopProductId,BrosShopUserId,BrosShopRating,BrosShopComment,BrosShopDateTime")] BrosShopReview brosShopReview)
        {
            if (id != brosShopReview.BrosShopReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brosShopReview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrosShopReviewExists(brosShopReview.BrosShopReviewId))
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
            ViewData["BrosShopProductId"] = new SelectList(_context.BrosShopProducts, "BrosShopProductId", "BrosShopProductId", brosShopReview.BrosShopProductId);
            ViewData["BrosShopUserId"] = new SelectList(_context.BrosShopUsers, "BrosShopUserId", "BrosShopUserId", brosShopReview.BrosShopUserId);
            return View(brosShopReview);
        }

        // GET: BrosShopReviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopReview = await _context.BrosShopReviews
                .Include(b => b.BrosShopProduct)
                .Include(b => b.BrosShopUser)
                .FirstOrDefaultAsync(m => m.BrosShopReviewId == id);
            if (brosShopReview == null)
            {
                return NotFound();
            }

            return View(brosShopReview);
        }

        // POST: BrosShopReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brosShopReview = await _context.BrosShopReviews.FindAsync(id);
            if (brosShopReview != null)
            {
                _context.BrosShopReviews.Remove(brosShopReview);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrosShopReviewExists(int id)
        {
            return _context.BrosShopReviews.Any(e => e.BrosShopReviewId == id);
        }
    }
}
