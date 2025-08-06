using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IBSVF.Web.Data;
using IBSVF.Web.Models;

namespace IBSVF.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmarParticipacao([FromBody] ParticipanteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dados inválidos" });
            }

            try
            {
                var participante = new Participante
                {
                    Nome = model.Name,
                    Comparecimento = model.Attendance,
                    DataCriacao = DateTime.UtcNow
                };

                _context.Participantes.Add(participante);
                await _context.SaveChangesAsync();

                // Adicionar acompanhantes se houver
                if (model.Companions?.Any() == true)
                {
                    foreach (var companionName in model.Companions.Where(c => !string.IsNullOrWhiteSpace(c)))
                    {
                        var acompanhante = new Acompanhante
                        {
                            ParticipanteId = participante.Id,
                            Nome = companionName.Trim()
                        };
                        _context.Acompanhantes.Add(acompanhante);
                    }
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro ao salvar confirmação" });
            }
        }
    }
}
