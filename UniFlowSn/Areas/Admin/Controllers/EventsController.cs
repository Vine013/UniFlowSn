using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniFlowSn.Models.Db;
using static System.Net.Mime.MediaTypeNames;

namespace UniFlowSn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class EventsController : Controller
    {
        private readonly UniFlowDbContext _context;

        public EventsController(UniFlowDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Events
        public async Task<IActionResult> Index()
        {
            var uniFlowDbContext = _context.Events.Include(e => e.Place).Include(e => e.Type);
            return View(await uniFlowDbContext.ToListAsync());
        }

        public IActionResult DeleteGallery(int id)
        {
            var gallery = _context.EventGalleries.FirstOrDefault(x => x.Id == id);
            if (gallery == null)
            {
                return NotFound();
            }
            string d = Directory.GetCurrentDirectory();
            string fn = d + "\\wwwroot\\images\\banners\\" + gallery.ImageName;

            if (System.IO.File.Exists(fn))
            {
                System.IO.File.Delete(fn);
            }
            _context.Remove(gallery);
            _context.SaveChanges();

            return Redirect("edit/" + gallery.EventId);
        }

        // GET: Admin/Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Place)
                .Include(e => e.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["PlaceId"] = new SelectList(_context.EventPlaces, "Id", "Place", @event.PlaceId);
            ViewData["TypeId"] = new SelectList(_context.EventTypes, "Id", "Type", @event.TypeId);
            return View(@event);
        }

        // GET: Admin/Events/Create
        public IActionResult Create()
        {
            ViewData["PlaceId"] = new SelectList(_context.EventPlaces, "Id", "Place");
            ViewData["TypeId"] = new SelectList(_context.EventTypes, "Id", "Type");
            return View();
        }

        // POST: Admin/Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,FullDescription,ImageName,Qty,Tags,VideoUrl,DtStart,DtEnd,PlaceId,TypeId")] Event @event, IFormFile? MainImage, IFormFile[]? GalleryImages)
        {
            if (ModelState.IsValid)
            {
                //============ SAVE MAIN IMAGE ===============
                if (MainImage != null)
                {
                    @event.ImageName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(MainImage.FileName);
                    string fn;
                    fn = Directory.GetCurrentDirectory();
                    string ImagePath = Path.Combine(fn + "\\wwwroot\\images\\banners\\" + @event.ImageName);

                    using (var stream = new FileStream(ImagePath, FileMode.Create))
                    {
                        MainImage.CopyTo(stream);
                    }
                }
                //============================================
                _context.Add(@event);
                await _context.SaveChangesAsync();
                //=========== SAVE GALLERY IMAGES ============
                if (GalleryImages != null)
                {
                    foreach (var item in GalleryImages)
                    {
                        var newgallery = new EventGallery();
                        newgallery.EventId = @event.Id;

                        newgallery.ImageName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(item.FileName);
                        string fn;
                        fn = Directory.GetCurrentDirectory();
                        string ImagePath = fn + "\\wwwroot\\images\\banners\\" + newgallery.ImageName;

                        using (var stream = new FileStream(ImagePath, FileMode.Create))
                        {
                            item.CopyTo(stream);
                        }

                        _context.EventGalleries.Add(newgallery);
                    }
                }
                await _context.SaveChangesAsync();
                //============================================
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlaceId"] = new SelectList(_context.EventPlaces, "Id", "Place", @event.PlaceId);
            ViewData["TypeId"] = new SelectList(_context.EventTypes, "Id", "Type", @event.TypeId);
            return View(@event);
        }

        // GET: Admin/Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            //=========ASSIGN CODE TO VIEW DATA=============
            ViewData["gallery"] = _context.EventGalleries
                .Where(x => x.EventId == @event.Id)
                .ToList();
            //==============================================
            ViewData["PlaceId"] = new SelectList(_context.EventPlaces, "Id", "Place", @event.PlaceId);
            ViewData["TypeId"] = new SelectList(_context.EventTypes, "Id", "Type", @event.TypeId);
            return View(@event);
        }

        // POST: Admin/Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,FullDescription,ImageName,Qty,Tags,VideoUrl,DtStart,DtEnd,PlaceId,TypeId")] Event @event, IFormFile? MainImage, IFormFile[]? GalleryImages)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //======== SAVE IMAGES ==========
                    if (MainImage != null)
                    {
                        string d = Directory.GetCurrentDirectory();
                        string fn = d + "\\wwwroot\\images\\banners\\" + @event.ImageName;

                        if (System.IO.File.Exists(fn))
                        {
                            System.IO.File.Delete(fn);
                        }

                        using (var stream = new FileStream(fn, FileMode.Create))
                        {
                            MainImage.CopyTo(stream);
                        }

                    }
                    if (GalleryImages != null)
                    {
                        foreach (var item in GalleryImages)
                        {

                            var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);

                            string d = Directory.GetCurrentDirectory();
                            string fn = d + "\\wwwroot\\images\\banners\\" + imageName;

                            using (var stream = new FileStream(fn, FileMode.Create))
                            {
                                item.CopyTo(stream);
                            }

                            var galleryItem = new EventGallery();
                            galleryItem.ImageName = imageName;
                            galleryItem.EventId = @event.Id;

                            _context.EventGalleries.Add(galleryItem);
                        }
                    }
                    //===============================
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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
            ViewData["PlaceId"] = new SelectList(_context.EventPlaces, "Id", "Place", @event.PlaceId);
            ViewData["TypeId"] = new SelectList(_context.EventTypes, "Id", "Type", @event.TypeId);
            return View(@event);
        }

        // GET: Admin/Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Place)
                .Include(e => e.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Admin/Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                //======== DELETE IMAGES ===========
                string d = Directory.GetCurrentDirectory();
                string fn = d + "\\wwwroot\\images\\banners\\";
                string mainImagePath = fn + @event.ImageName;
                if (System.IO.File.Exists(mainImagePath))
                {
                    System.IO.File.Delete(mainImagePath);
                }
                var galleries = _context.EventGalleries
                    .Where(x => x.EventId == id)
                    .ToList();
                if (galleries != null)
                {
                    foreach (var item in galleries)
                    {
                        string galleryImagePath = fn + item.ImageName;
                        if (System.IO.File.Exists(galleryImagePath))
                        {
                            System.IO.File.Delete(galleryImagePath);
                        }
                    }
                    _context.EventGalleries.RemoveRange(galleries);
                }
                //==================================
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
