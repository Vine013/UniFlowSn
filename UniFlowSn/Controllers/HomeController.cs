using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniFlowSn.Models;
using UniFlowSn.Models.Db;

namespace UniFlowSn.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UniFlowDbContext _context;

        public HomeController(ILogger<HomeController> logger, UniFlowDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        #region Index
        public IActionResult Index()
        {
            var banners = _context.Banners.Where(b => b.Position == "Slider").OrderBy(x => x.Priority).ToList();
            ViewData["banners"] = banners;

            var eventosPorTipo = _context.Events
                .Include(e => e.Type)
                .Include(e => e.Place)
                .OrderBy(e => e.DtStart)
                .GroupBy(e => e.TypeId)
                .Select(g => new
                {
                    TypeId = g.Key,
                    TypeName = g.First().Type == null ? null : g.First().Type.Type,
                    Events = g.ToList()
                })
                .ToList();

            ViewBag.EventosPorTipo = eventosPorTipo;

            return View();
        }
        #endregion

        #region About
        public IActionResult About()
        {
            return View("~/Views/About/Index.cshtml");
        }

        #endregion

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
