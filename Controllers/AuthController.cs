using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using IBSVF.Web.Data;
using IBSVF.Web.Models;

namespace IBSVF.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Se já estiver logado, redireciona para o dashboard
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                // Log para debug
                Console.WriteLine($"Tentativa de login - Username: {model?.Username}, ModelState Valid: {ModelState.IsValid}");
                
                if (model == null)
                {
                    return Json(new { success = false, message = "Dados não recebidos" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return Json(new { success = false, message = $"Dados inválidos: {errors}" });
                }

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

                Console.WriteLine($"Usuário encontrado: {usuario != null}");

                if (usuario != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Username),
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Dashboard") });
                }

                return Json(new { success = false, message = "Usuário ou senha incorretos!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no login: {ex.Message}");
                return Json(new { success = false, message = "Erro interno do servidor: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
