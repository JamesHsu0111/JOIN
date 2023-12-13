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
    public class PersonalPhotosController : Controller
    {
        private readonly ProjectXContext _context;

        public PersonalPhotosController(ProjectXContext context)
        {
            _context = context;
        }

        // GET: PersonalPhotos PersonalPhoto
        public async Task<IActionResult> Index()
        {
            var projectXContext = _context.PersonalPhoto.Include(p => p.Group);
            return View(await projectXContext.ToListAsync());
        }

        // GET: PersonalPhotos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PersonalPhoto == null)
            {
                return NotFound();
            }

            var personalPhoto = await _context.PersonalPhoto
                .Include(p => p.Group)
                .FirstOrDefaultAsync(m => m.PersonalPhotoID == id);
            if (personalPhoto == null)
            {
                return NotFound();
            }

            return View(personalPhoto);
        }

        // GET: PersonalPhotos/Create
        public IActionResult Create()
        {
            ViewData["GroupID"] = new SelectList(_context.Group, "GroupID", "GroupID");
            return View();
        }

        // POST: PersonalPhotos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonalPhotoID,GroupID,PhotoData")] PersonalPhoto personalPhoto, List<IFormFile> imageDataFiles)
        {
            if (imageDataFiles != null && imageDataFiles.Count > 0)
            {
                foreach (var imageDataFile in imageDataFiles)
                {
                    if (imageDataFile != null && imageDataFile.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await imageDataFile.CopyToAsync(stream);

                            // 創建一個新的 PersonalPhoto 對象，設置 GroupID 和 PhotoData，然後保存到資料庫
                            var newPhoto = new PersonalPhoto
                            {
                                GroupID = personalPhoto.GroupID,
                                PhotoData = stream.ToArray() // 將圖片資料存入 PhotoData
                            };

                            // 將 PersonalPhoto 存入資料庫
                            _context.PersonalPhoto.Add(newPhoto);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(personalPhoto);
        }


        // GET: PersonalPhotos/Edit/5
        public async Task<IActionResult> Edit(int? id)
            {
                if (id == null || _context.PersonalPhoto == null)
                {
                    return NotFound();
                }

                var personalPhoto = await _context.PersonalPhoto.FindAsync(id);
                if (personalPhoto == null)
                {
                    return NotFound();
                }
                ViewData["GroupID"] = new SelectList(_context.Group, "GroupID", "GroupID", personalPhoto.GroupID);
                return View(personalPhoto);
            }

            // POST: PersonalPhotos/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("PersonalPhotoID,GroupID,PhotoData")] PersonalPhoto personalPhoto)
            {
                if (id != personalPhoto.PersonalPhotoID)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(personalPhoto);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PersonalPhotoExists(personalPhoto.PersonalPhotoID))
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
                ViewData["GroupID"] = new SelectList(_context.Group, "GroupID", "GroupID", personalPhoto.GroupID);
                return View(personalPhoto);
            }

            // GET: PersonalPhotos/Delete/5
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null || _context.PersonalPhoto == null)
                {
                    return NotFound();
                }

                var personalPhoto = await _context.PersonalPhoto
                    .Include(p => p.Group)
                    .FirstOrDefaultAsync(m => m.PersonalPhotoID == id);
                if (personalPhoto == null)
                {
                    return NotFound();
                }

                return View(personalPhoto);
            }

            // POST: PersonalPhotos/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                if (_context.PersonalPhoto == null)
                {
                    return Problem("Entity set 'ProjectXContext.PersonalPhoto'  is null.");
                }
                var personalPhoto = await _context.PersonalPhoto.FindAsync(id);
                if (personalPhoto != null)
                {
                    _context.PersonalPhoto.Remove(personalPhoto);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool PersonalPhotoExists(int id)
            {
                return (_context.PersonalPhoto?.Any(e => e.PersonalPhotoID == id)).GetValueOrDefault();
            }
        }
    }
