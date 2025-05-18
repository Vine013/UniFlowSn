using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UniFlowSn.Models.Db;

namespace UniFlowSn.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class EventPlacesController : Controller
    {
        private readonly UniFlowDbContext _context;

        public EventPlacesController(UniFlowDbContext context)
        {
            _context = context;
        }

        // GET: Admin/EventPlaces
        public async Task<IActionResult> Index()
        {
            return View(await _context.EventPlaces.ToListAsync());
        }

        // GET: Admin/EventPlaces/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventPlace = await _context.EventPlaces
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventPlace == null)
            {
                return NotFound();
            }

            return View(eventPlace);
        }

        // GET: Admin/EventPlaces/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/EventPlaces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Place,Address,City,State")] EventPlace eventPlace)
        {
            if (ModelState.IsValid)
            {
                _context.Add(eventPlace);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eventPlace);
        }

        // GET: Admin/EventPlaces/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventPlace = await _context.EventPlaces.FindAsync(id);
            if (eventPlace == null)
            {
                return NotFound();
            }
            return View(eventPlace);
        }

        // POST: Admin/EventPlaces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Place,Address,City,State")] EventPlace eventPlace)
        {
            if (id != eventPlace.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventPlace);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventPlaceExists(eventPlace.Id))
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
            return View(eventPlace);
        }

        // GET: Admin/EventPlaces/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventPlace = await _context.EventPlaces
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eventPlace == null)
            {
                return NotFound();
            }

            return View(eventPlace);
        }

        // POST: Admin/EventPlaces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventPlace = await _context.EventPlaces.FindAsync(id);
            if (eventPlace != null)
            {
                _context.EventPlaces.Remove(eventPlace);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventPlaceExists(int id)
        {
            return _context.EventPlaces.Any(e => e.Id == id);
        }
    }
}
