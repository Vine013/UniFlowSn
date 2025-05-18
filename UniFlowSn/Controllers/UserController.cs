using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UniFlowSn.Models.Db;
using System.Collections.Generic;
using UniFlowSn.Models.ViewModels;

namespace UniFlowSn.Controllers
{
    [Authorize] // Garante que apenas usuários logados possam acessar esta página
    public class UserController : Controller
    {
        private readonly UniFlowDbContext _context;

        public UserController(UniFlowDbContext context)
        {
            _context = context;
        }

        #region Home
        public async Task<IActionResult> Home()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        #endregion

        #region My Events
        public async Task<IActionResult> MyEvents()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToAction("Login", "Account"); // Redireciona se não conseguir obter o ID
            }

            var eventosInscritos = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Event)
                    .ThenInclude(e => e.Place)
                .Select(o => o.Event)
                .ToListAsync();

            return View(eventosInscritos);
        }
        #endregion

        #region Inscription Details
        public async Task<IActionResult> InscriptionDetails(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var inscricao = await _context.Orders
                .Where(o => o.UserId == userId && o.EventId == id)
                .Include(o => o.Event)
                    .ThenInclude(e => e.Place)
                .FirstOrDefaultAsync();

            if (inscricao == null || inscricao.Event == null)
            {
                return NotFound();
            }

            var usuario = await _context.Users.FindAsync(userId);

            if (usuario == null)
            {
                return NotFound();
            }

            var ticketViewModel = new TicketViewModel
            {
                Event = inscricao.Event.Title,
                DtStart = inscricao.Event.DtStart,
                Place = inscricao.Event.Place?.Place,
                User = $"{usuario.FirstName} {usuario.LastName}",
                Email = usuario.Email,
                CreateDate = inscricao.CreateDate,
                Id = Guid.NewGuid().ToString().Substring(0, 8).ToUpper() // Você pode querer usar um ID mais significativo se tiver
            };

            return View("~/Views/Inscription/Ticket.cshtml", ticketViewModel);
        }
        #endregion
    }
}