using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.RegularExpressions;
using UniFlowSn.Models.Db;
using UniFlowSn.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Net.Mail;

namespace UniFlowSn.Controllers
{
    public class AccountController : Controller
    {
        private UniFlowDbContext _context;

        #region Register
        public AccountController(UniFlowDbContext context)
        {
            _context = context;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            user.RegisterDate = DateTime.Now;
            user.IsAdmin = false;
            user.Email = user.Email?.Trim();
            user.Password = user.Password?.Trim();
            user.FirstName = user.FirstName?.Trim();
            user.LastName = user.LastName?.Trim();
            user.Phone = user.Phone?.Trim();
            user.Address = user.Address?.Trim();
            user.RecoveryCode = 0;
            //------------------------
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            //------------Valid Email Checking------------
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(user.Email);
            if (!match.Success)
            {
                ModelState.AddModelError("Email", "E-mail inválido!");
                return View(user);
            }
            //-----------Duplicate Email Checking-------------
            var prevUser = _context.Users.Any(x => x.Email == user.Email);
            if (prevUser == true)
            {
                ModelState.AddModelError("Email", "E-mail já cadastrado!");
                return View(user);
            }
            //------------------------------------------------
            _context.Users.Add(user);
            _context.SaveChanges();
            //------------------------
            return RedirectToAction("login");
        }
        #endregion

        #region Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel user)
        {
            //------------
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            //------------
            var foundUser = _context.Users.FirstOrDefault(x => x.Email == user.Email.Trim() && x.Password == user.Password.Trim());
            //-----
            if (foundUser == null)
            {
                ModelState.AddModelError("Email", "E-mail ou senha incorretos!");
                return View(user);
            }
            //------------
            // Create claims for the authenticated user
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, foundUser.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, foundUser.FirstName));
            claims.Add(new Claim(ClaimTypes.Surname, foundUser.LastName));
            claims.Add(new Claim(ClaimTypes.MobilePhone, foundUser.Phone));
            claims.Add(new Claim(ClaimTypes.Email, foundUser.Email));
            //------------
            if (foundUser.IsAdmin == true)
            {
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "user"));
            }
            //------------
            // Create an identity based on the claims
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            //------------
            // Create a principal based on the identity
            var principal = new ClaimsPrincipal(identity);
            //------------
            // Sign in the user with the created principal
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            //------------
            return Redirect("/");
        }
        #endregion

        #region Logout
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Recovery Password
        public IActionResult RecoveryPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RecoveryPassword(RecoveryPasswordViewModel recoveryPassword)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            ////-------------------------------------------

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(recoveryPassword.Email);
            if (!match.Success)
            {
                ModelState.AddModelError("Email", "E-mail Inválido!");
                return View(recoveryPassword);
            }

            ////-------------------------------------------

            var foundUser = _context.Users.FirstOrDefault(x => x.Email == recoveryPassword.Email.Trim());
            if (foundUser == null)
            {
                ModelState.AddModelError("Email", "E-mail Inexistente!");
                return View(recoveryPassword);
            }

            ////-------------------------------------------

            foundUser.RecoveryCode = new Random().Next(10000, 100000);
            _context.Users.Update(foundUser);
            _context.SaveChanges();

            ////-------------------------------------------

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("dataflow.ia@gmail.com");
            mail.To.Add(foundUser.Email);
            mail.Subject = "UniFlow  - Código de Recuperação";
            mail.IsBodyHtml = true;

            string htmlBody = $@"
                                <html>
                                  <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                                    <div style='max-width: 600px; margin: auto; background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);'>
                                      <h2 style='color: #0FC2C0;'>Olá,</h2>
                                      <p style='font-size: 16px; color: #333333;'>
                                        Recebemos uma solicitação para redefinir sua senha na plataforma <strong>UniFlow</strong>.
                                        Use o código de recuperação abaixo para prosseguir com a redefinição:
                                      </p>
                                      <div style='text-align: center; margin: 30px 0;'>
                                        <span style='display: inline-block; background-color: #0FC2C0; color: #ffffff; font-size: 24px; padding: 15px 25px; border-radius: 5px; letter-spacing: 3px;'>
                                          {foundUser.RecoveryCode}
                                        </span>
                                      </div>
                                      <p style='font-size: 14px; color: #999999;'>
                                        Se você não solicitou essa alteração, ignore este e-mail.
                                      </p>
                                      <p style='font-size: 14px; color: #999999; margin-top: 30px;'>
                                        Atenciosamente,<br>
                                        DataFlow Team
                                      </p>
                                    </div>
                                  </body>
                                </html>";
            mail.Body = htmlBody;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("dataflow.ia@gmail.com", "bexy dhba aaiw mtov");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);

            ////-------------------------------------------
            return Redirect("/Account/ResetPassword?email=" + foundUser.Email);
        }

        #endregion

        #region Reset Password

        public IActionResult ResetPassword(string email)
        {
            var resetPasswordModel = new ResetPasswordViewModel();
            resetPasswordModel.Email = email;

            return View(resetPasswordModel);
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel resetPassword)
        {
            if (!ModelState.IsValid)
            {
                return View(resetPassword);
            }

            ////-------------------------------------------

            var foundUser = _context.Users.FirstOrDefault(x => x.Email == resetPassword.Email && x.RecoveryCode == resetPassword.RecoveryCode);
            if (foundUser == null)
            {
                ModelState.AddModelError("RecoveryCode", "O Código de Recuperação está incorreto!");
                return View(resetPassword);
            }

            ////-------------------------------------------

            foundUser.Password = resetPassword.NewPassword;

            _context.Users.Update(foundUser);
            _context.SaveChanges();

            ////-------------------------------------------

            return RedirectToAction("Login");
        }

        #endregion
    }
}
