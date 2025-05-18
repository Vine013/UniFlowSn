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
        public IActionResult Index()
        {
            List<Event> events = _context.Events
                .Include(e => e.Type)
                .Include(e => e.Place)
                .OrderByDescending(x => x.Id).ToList();
            return View(events);
        }
        public IActionResult SearchProducts(string SearchText)
        {
            var events = _context.Events
                .Where(x => EF.Functions.Like(x.Title, "%" + SearchText + "%") ||
                EF.Functions.Like(x.Tags, "%" + SearchText + "%"))
                .OrderBy(x => x.Title)
                .ToList();
            return View("Index", events);
        }
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
                    return Redirect("/Products/ProductDetails/" + eventId);
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
                return Redirect("/Products/ProductDetails/" + eventId);
            }
            else
            {
                TempData["ErrorMessage"] = "Por favor, preencha todos os campos!";
                return Redirect("/Products/ProductDetails/" + eventId);
            }

        }

    }
}
