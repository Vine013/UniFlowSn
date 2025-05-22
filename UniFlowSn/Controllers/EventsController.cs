using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using UniFlowSn.Models.Db;

namespace UniFlowSn.Controllers
{
    public class EventsController : Controller
    {
        private readonly UniFlowDbContext _context;
        public EventsController(UniFlowDbContext context)
        {
            _context = context;
        }

        #region Index
        public IActionResult Index(int? typeId, string? localId, DateTime? startDate, DateTime? endDate, string? searchTerm)
        {
            IQueryable<Event> events = _context.Events
                .Include(e => e.Place)
                .Include(e => e.Type)
                .OrderByDescending(x => x.DtStart);

            // Lógica de filtro por typeId
            if (typeId.HasValue)
            {
                events = events.Where(e => e.TypeId == typeId.Value);
            }

            // Lógica de filtro por localId
            if (!string.IsNullOrEmpty(localId))
            {
                events = events.Where(e => e.PlaceId.ToString() == localId);
            }

            // Lógica de filtro por startDate
            if (startDate.HasValue)
            {
                events = events.Where(e => e.DtStart >= startDate.Value);
            }

            // Lógica de filtro por endDate
            if (endDate.HasValue)
            {
                events = events.Where(e => e.DtEnd <= endDate.Value);
            }

            // Lógica de filtro por searchTerm (busca em Título e Tags)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                events = events.Where(e => EF.Functions.Like(e.Title, "%" + searchTerm + "%") ||
                                           EF.Functions.Like(e.Tags, "%" + searchTerm + "%"));
            }

            return View(events.ToList());
        }
        #endregion

        #region SearchProducts
        public IActionResult SearchProducts(string SearchText)
        {
            var events = _context.Events
                .Where(x => EF.Functions.Like(x.Title, "%" + SearchText + "%") ||
                EF.Functions.Like(x.Tags, "%" + SearchText + "%"))
                .OrderBy(x => x.Title)
                .ToList();
            return View("Index", events);
        }
        #endregion

        #region EventDetails
        public IActionResult EventDetails(int id)
        {
            Event? @event = _context.Events.FirstOrDefault(x => x.Id == id);
            if (@event == null)
            {
                return NotFound();
            }
            //Galeria de Fotos
            ViewData["gallery"] = _context.EventGalleries
                .Where(x => x.EventId == id)
                .ToList();
            //Carrossel de Novos Produtos
            ViewData["NewProducts"] = _context.Events
                .Where(x => x.Id != id)
                .Take(10)
                .OrderByDescending(x => x.Id)
                .ToList();
            //Comentários de Reviews
            ViewData["comments"] = _context.Comments
                .Where(x => x.EventId == id)
                .OrderByDescending(x => x.CreateDate)
                .ToList();
            return View(@event);
        }
        #endregion

        #region Comments
        [HttpPost]
        public IActionResult SubmitComment(string name, string email, string comment, int eventId)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(comment) && eventId != 0)
            {
                //REGEX - Validação de E-mail
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(email);
                if (!match.Success)
                {
                    TempData["ErrorMessage"] = "Email inválido";
                    return Redirect("/Events/EventDetails/" + eventId);
                }

                Comment newComment = new Comment();
                newComment.Name = name;
                newComment.Email = email;
                newComment.CommentText = comment;
                newComment.EventId = eventId;
                newComment.CreateDate = DateTime.Now;

                _context.Comments.Add(newComment);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Seu comentário foi enviado com sucesso!";
                return Redirect("/Events/EventDetails/" + eventId);
            }
            else
            {
                TempData["ErrorMessage"] = "Por favor, preencha todos os campos!";
                return Redirect("/Events/EventDetails/" + eventId);
            }
        }
        #endregion

    }
}
