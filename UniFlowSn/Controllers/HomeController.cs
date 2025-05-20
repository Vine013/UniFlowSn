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

            var eventTypes = _context.EventTypes.OrderBy(t => t.Type).ToList();
            ViewBag.EventTypes = eventTypes;

            var upcomingEventsHome = _context.Events
                .Include(e => e.Place)
                .Where(e => e.DtStart > DateTime.Now) // Filtra eventos futuros
                .OrderBy(e => e.DtStart) // Ordena pelo DtStart mais próximo
                .Take(9) // Pega os 9 eventos mais próximos
                .ToList();

            ViewBag.LatestEvents = upcomingEventsHome;

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
