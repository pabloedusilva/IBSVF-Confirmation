using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using IBSVF.Web.Data;
using IBSVF.Web.Models;

namespace IBSVF.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var participantes = await _context.Participantes
                .Include(p => p.Acompanhantes)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                ConfirmedCount = participantes.Count(p => p.Comparecimento == "yes"),
                NotConfirmedCount = participantes.Count(p => p.Comparecimento == "no"),
                TotalPeople = participantes.Count + participantes.SelectMany(p => p.Acompanhantes).Count(),
                FamiliesCount = participantes.Count(p => p.Comparecimento == "yes"),
                Participantes = participantes.Select(p => new ParticipanteDetalhado
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Comparecimento = p.Comparecimento,
                    DataCriacao = p.DataCriacao,
                    Acompanhantes = p.Acompanhantes.Select(a => a.Nome).ToList()
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetParticipants()
        {
            var participantes = await _context.Participantes
                .Include(p => p.Acompanhantes)
                .OrderByDescending(p => p.DataCriacao)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Nome,
                    attendance = p.Comparecimento,
                    date = p.DataCriacao.ToString("dd/MM/yyyy HH:mm"),
                    companions = p.Acompanhantes.Select(a => a.Nome).ToList()
                })
                .ToListAsync();

            return Json(participantes);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateParticipant([FromBody] dynamic data)
        {
            try
            {
                int id = data.id;
                string name = data.name;
                string attendance = data.attendance;
                List<string> companions = data.companions?.ToObject<List<string>>() ?? new List<string>();

                var participante = await _context.Participantes
                    .Include(p => p.Acompanhantes)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (participante == null)
                {
                    return Json(new { success = false, message = "Participante não encontrado" });
                }

                participante.Nome = name;
                participante.Comparecimento = attendance;

                // Remover acompanhantes existentes
                _context.Acompanhantes.RemoveRange(participante.Acompanhantes);

                // Adicionar novos acompanhantes
                foreach (var companionName in companions.Where(c => !string.IsNullOrWhiteSpace(c)))
                {
                    participante.Acompanhantes.Add(new Acompanhante
                    {
                        Nome = companionName.Trim(),
                        ParticipanteId = participante.Id
                    });
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro ao atualizar participante" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteParticipant(int id)
        {
            try
            {
                var participante = await _context.Participantes
                    .Include(p => p.Acompanhantes)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (participante == null)
                {
                    return Json(new { success = false, message = "Participante não encontrado" });
                }

                _context.Participantes.Remove(participante);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro ao excluir participante" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportCsv()
        {
            var participantes = await _context.Participantes
                .Include(p => p.Acompanhantes)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            var csv = "Nome,Status,Acompanhantes,Data\n";
            foreach (var p in participantes)
            {
                var acompanhantes = string.Join(";", p.Acompanhantes.Select(a => a.Nome));
                var status = p.Comparecimento == "yes" ? "Confirmado" : "Não Confirmado";
                csv += $"{p.Nome},{status},\"{acompanhantes}\",{p.DataCriacao:dd/MM/yyyy HH:mm}\n";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            var fileName = $"participantes_family_day_{DateTime.Now:dd-MM-yyyy}.csv";

            return File(bytes, "text/csv", fileName);
        }
    }
}
