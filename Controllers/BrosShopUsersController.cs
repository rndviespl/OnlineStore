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
    public class BrosShopUsersController : Controller
    {
        private readonly ApplicationContext _context;

        public BrosShopUsersController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: BrosShopUsers
        public async Task<IActionResult> Index()
        {
            return View(await _context.BrosShopUsers.ToListAsync());
        }

        // GET: BrosShopUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopUser = await _context.BrosShopUsers
                .FirstOrDefaultAsync(m => m.BrosShopUserId == id);
            if (brosShopUser == null)
            {
                return NotFound();
            }

            return View(brosShopUser);
        }

        // GET: BrosShopUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BrosShopUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BrosShopUserId,BrosShopUsername,BrosShopPassword,BrosShopEmail,BrosShopFullName,BrosShopRegistrationDate,BrosShopPhoneNumber")] BrosShopUser brosShopUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brosShopUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brosShopUser);
        }

        // GET: BrosShopUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopUser = await _context.BrosShopUsers.FindAsync(id);
            if (brosShopUser == null)
            {
                return NotFound();
            }
            return View(brosShopUser);
        }

        // POST: BrosShopUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BrosShopUserId,BrosShopUsername,BrosShopPassword,BrosShopEmail,BrosShopFullName,BrosShopRegistrationDate,BrosShopPhoneNumber")] BrosShopUser brosShopUser)
        {
            if (id != brosShopUser.BrosShopUserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brosShopUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrosShopUserExists(brosShopUser.BrosShopUserId))
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
            return View(brosShopUser);
        }

        // GET: BrosShopUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brosShopUser = await _context.BrosShopUsers
                .FirstOrDefaultAsync(m => m.BrosShopUserId == id);
            if (brosShopUser == null)
            {
                return NotFound();
            }

            return View(brosShopUser);
        }

        // POST: BrosShopUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brosShopUser = await _context.BrosShopUsers.FindAsync(id);
            if (brosShopUser != null)
            {
                _context.BrosShopUsers.Remove(brosShopUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrosShopUserExists(int id)
        {
            return _context.BrosShopUsers.Any(e => e.BrosShopUserId == id);
        }
    }
}
