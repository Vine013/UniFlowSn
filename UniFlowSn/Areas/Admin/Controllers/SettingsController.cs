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
    public class SettingsController : Controller
    {
        private readonly UniFlowDbContext _context;

        public SettingsController(UniFlowDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Settings/Edit/5
        public async Task<IActionResult> Edit()
        {
            var setting = await _context.Settings.FirstAsync();
            if (setting == null)
            {
                return NotFound();
            }
            return View(setting);
        }

        // POST: Admin/Settings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Address,Email,Phone,CopyRight,Instagram,Facebook,Youtuber,Twitter,Logo")] Setting setting, IFormFile? newLogo)
        {
            if (id != setting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (newLogo != null)
                    {

                        string d = Directory.GetCurrentDirectory();
                        string path = d + "\\wwwroot\\images\\" + setting.Logo;
                        //------------------------------------------------
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }

                        //------------------------------------------------
                        setting.Logo = Guid.NewGuid() + Path.GetExtension(newLogo.FileName);
                        path = d + "\\wwwroot\\images\\" + setting.Logo;
                        //------------------------------------------------

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            newLogo.CopyTo(stream);
                        }
                    }

                    _context.Update(setting);
                    await _context.SaveChangesAsync();

                    TempData["message"] = "Configurações Atualizadas!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SettingExists(setting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return Redirect($"/admin/Settings/Edit");
        }


        private bool SettingExists(int id)
        {
            return _context.Settings.Any(e => e.Id == id);
        }
    }
}
