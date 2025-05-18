using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;
using UniFlowSn.Models.Db;
using UniFlowSn.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;



namespace UniFlowSn.Controllers
{
    public class InscriptionController : Controller
    {
        private readonly UniFlowDbContext _context;

        public InscriptionController(UniFlowDbContext context)
        {
            _context = context;
        }

        #region Inscribe
        [Authorize]
        public IActionResult Inscribe(int id)
        {
            // 1. Obter o ID do usuário logado
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                // Se não conseguir obter o ID do usuário, algo está errado com a autenticação
                return RedirectToAction("Login", "Account"); // Redireciona para a página de login
            }

            // 2. Buscar as informações do usuário logado
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                // Usuário não encontrado (erro inesperado)
                return NotFound();
            }

            // 3. Criar uma instância do InscricaoViewModel e preencher com os dados do usuário
            var inscricaoViewModel = new InscriptionViewModel
            {
                EventId = id,
                UserId = userId,
                Name = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address
            };

            // 4. Retornar a View Inscrever, passando o ViewModel preenchido
            return View(inscricaoViewModel);
        }
        #endregion

        #region Confirm Inscription
        [HttpPost]
        [Authorize]
        public IActionResult ConfirmInscription(InscriptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Obter o ID do usuário logado
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                // 2. Verificar se o usuário já está inscrito neste evento
                var existingInscription = _context.Orders.FirstOrDefault(o => o.UserId == userId && o.EventId == model.EventId);

                if (existingInscription != null)
                {
                    TempData["ErrorMessage"] = "Você já está inscrito neste evento.";
                    return RedirectToAction("EventDetails", "Events", new { id = model.EventId });
                }
                else
                {
                    // 3. Buscar o evento para atualizar a quantidade
                    var evento = _context.Events
                        .Include(e => e.Place)
                        .FirstOrDefault(e => e.Id == model.EventId);

                    if (evento != null && evento.Qty > 0)
                    {
                        // 4. Decrementar a quantidade
                        evento.Qty--;
                        _context.Events.Update(evento);

                        // 5. Criar uma nova inscrição
                        var inscription = new Order
                        {
                            UserId = userId,
                            EventId = model.EventId,
                            CreateDate = DateTime.Now,
                            Status = "Inscrito"
                        };
                        _context.Orders.Add(inscription);
                        _context.SaveChanges(); // Salva a inscrição E a atualização da quantidade

                        var usuario = _context.Users.FirstOrDefault(u => u.Id == userId);

                        if (usuario != null)
                        {
                            var ticketViewModel = new TicketViewModel
                            {
                                Event = evento.Title,
                                DtStart = evento.DtStart,
                                Place = evento.Place != null ? evento.Place.Place : "Local não especificado",
                                User = model.Name,
                                Email = model.Email,
                                CreateDate = inscription.CreateDate,
                                Id = Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                            };

                            // Enviar o e-mail
                            // Enviar o e-mail automático 
                            try
                            {
                                MailMessage mail = new MailMessage();
                                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                                mail.From = new MailAddress("dataflow.ia@gmail.com");
                                mail.To.Add(model.Email); // E-mail do participante
                                mail.Subject = $"Confirmação de Inscrição - {evento.Title}";
                                mail.IsBodyHtml = true;

                                string htmlBody = $@"
                                                     <!DOCTYPE html>
                                                     <html lang='pt-BR'>
                                                     <head>
                                                         <meta charset='UTF-8'>
                                                         <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                                         <title>Confirmação de Inscrição</title>
                                                         <style>
                                                             body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
                                                             .container {{ max-width: 600px; margin: 20px auto; background-color: #fff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1); }}
                                                             h2 {{ color: #007bff; margin-top: 0; }}
                                                             p {{ font-size: 16px; color: #333; line-height: 1.6; margin-bottom: 15px; }}
                                                             .event-details {{ margin-bottom: 20px; border-left: 3px solid #007bff; padding-left: 15px; }}
                                                             .inscription-details {{ margin-bottom: 20px; border-left: 3px solid #28a745; padding-left: 15px; }}
                                                             .ticket-code {{ background-color: #e9ecef; padding: 10px; border-radius: 5px; font-size: 1.1em; font-weight: bold; color: #000; text-align: center; margin-bottom: 20px; }}
                                                             .button {{ display: inline-block; background-color: #007bff; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px; margin-top: 10px; }}
                                                             .button:hover {{ background-color: #0056b3; }}
                                                             .footer {{ margin-top: 30px; text-align: center; color: #777; font-size: 14px; }}
                                                         </style>
                                                     </head>
                                                     <body>
                                                         <div class='container'>
                                                             <h2>Confirmação de Inscrição</h2>
                                                             <p>Olá, <strong>{model.Name}</strong>,</p>
                                                             <p>Sua inscrição no evento <strong>{evento.Title}</strong> foi confirmada com sucesso!</p>

                                                             <div class='event-details'>
                                                                 <h3>Detalhes do Evento:</h3>
                                                                 <p><strong>Nome:</strong> {evento.Title}</p>
                                                                 <p><strong>Data:</strong> {evento.DtStart.Value.ToString("dd/MM/yyyy HH:mm")}</p>
                                                                 <p><strong>Local:</strong> {ticketViewModel.Place}</p>
                                                             </div>

                                                             <div class='inscription-details'>
                                                                 <h3>Seus Dados de Inscrição:</h3>
                                                                 <p><strong>Nome:</strong> {model.Name}</p>
                                                                 <p><strong>E-mail:</strong> {model.Email}</p>
                                                             </div>

                                                             <div class='ticket-code'>
                                                                 <strong>Código do Ingresso:</strong> {ticketViewModel.Id}
                                                             </div>
                                                             <p>Guarde este código para acesso ao evento.</p>

                                                             <p>Agradecemos sua participação!</p>
                                                             <p>Atenciosamente,<br>Equipe UniFlow</p>

                                                             <div class='footer'>
                                                                 <p>Este é um e-mail automático. Por favor, não responda.</p>
                                                             </div>
                                                         </div>
                                                     </body>
                                                     </html>";

                                mail.Body = htmlBody;

                                SmtpServer.Port = 587; // Porta padrão para TLS
                                SmtpServer.Credentials = new System.Net.NetworkCredential("dataflow.ia@gmail.com", "bexy dhba aaiw mtov");
                                SmtpServer.EnableSsl = true;


                                SmtpServer.Send(mail);
                            }
                            catch (Exception ex)
                            {
                                // Log de erro: É importante registrar erros de envio de e-mail
                                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
                            }
                            return View("Ticket", ticketViewModel);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                    else if (evento != null && evento.Qty <= 0)
                    {
                        TempData["ErrorMessage"] = "Este evento atingiu o limite de inscrições.";
                        return RedirectToAction("EventDetails", "Events", new { id = model.EventId });
                    }
                    else
                    {
                        return NotFound(); // Evento não encontrado
                    }
                }
            }

            return View("Inscribe", model);
        }
        #endregion
    }
}
