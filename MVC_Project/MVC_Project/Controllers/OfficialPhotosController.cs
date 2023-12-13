using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_Project.Models;

namespace MVC_Project.Controllers
{
    public class OfficialPhotosController : Controller
    {
        private readonly ProjectXContext _context;

        public OfficialPhotosController(ProjectXContext context)
        {
            _context = context;
        }

        // GET: OfficialPhotoes
        public async Task<IActionResult> Index()
        {
            var projectXContext = _context.OfficialPhoto.Include(o => o.Activity);
            return View(await projectXContext.ToListAsync());
        }

        // GET: OfficialPhotoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OfficialPhoto == null)
            {
                return NotFound();
            }

            var officialPhoto = await _context.OfficialPhoto
                .Include(o => o.Activity)
                .FirstOrDefaultAsync(m => m.OfficialPhotoID == id);
            if (officialPhoto == null)
            {
                return NotFound();
            }

            return View(officialPhoto);
        }

        // GET: OfficialPhotoes/Create
        public IActionResult Create()
        {
            ViewData["ActivityID"] = new SelectList(_context.MyActivity, "ActivityID", "ActivityID");
            return View();
        }

        // POST: OfficialPhotoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OfficialPhotoID,ActivityID,PhotoPath")] OfficialPhoto officialPhoto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(officialPhoto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityID"] = new SelectList(_context.MyActivity, "ActivityID", "ActivityID", officialPhoto.ActivityID);
            return View(officialPhoto);
        }

        // GET: OfficialPhotoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OfficialPhoto == null)
            {
                return NotFound();
            }

            var officialPhoto = await _context.OfficialPhoto.FindAsync(id);
            if (officialPhoto == null)
            {
                return NotFound();
            }
            ViewData["ActivityID"] = new SelectList(_context.MyActivity, "ActivityID", "ActivityID", officialPhoto.ActivityID);
            return View(officialPhoto);
        }

        // POST: OfficialPhotoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OfficialPhotoID,ActivityID,PhotoPath")] OfficialPhoto officialPhoto)
        {
            if (id != officialPhoto.OfficialPhotoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(officialPhoto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfficialPhotoExists(officialPhoto.OfficialPhotoID))
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
            ViewData["ActivityID"] = new SelectList(_context.MyActivity, "ActivityID", "ActivityID", officialPhoto.ActivityID);
            return View(officialPhoto);
        }

        // GET: OfficialPhotoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OfficialPhoto == null)
            {
                return NotFound();
            }

            var officialPhoto = await _context.OfficialPhoto
                .Include(o => o.Activity)
                .FirstOrDefaultAsync(m => m.OfficialPhotoID == id);
            if (officialPhoto == null)
            {
                return NotFound();
            }

            return View(officialPhoto);
        }

        // POST: OfficialPhotoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OfficialPhoto == null)
            {
                return Problem("Entity set 'ProjectXContext.OfficialPhoto'  is null.");
            }
            var officialPhoto = await _context.OfficialPhoto.FindAsync(id);
            if (officialPhoto != null)
            {
                _context.OfficialPhoto.Remove(officialPhoto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OfficialPhotoExists(int id)
        {
          return (_context.OfficialPhoto?.Any(e => e.OfficialPhotoID == id)).GetValueOrDefault();
        }
    }
}
