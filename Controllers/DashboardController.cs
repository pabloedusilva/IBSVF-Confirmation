using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using IBSVF.Web.Data;
using IBSVF.Web.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

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
        public async Task<IActionResult> UpdateParticipant([FromBody] UpdateParticipantRequest request)
        {
            try
            {
                if (request == null)
                {
                    return Json(new { success = false, message = "Dados inválidos" });
                }

                var participante = await _context.Participantes
                    .Include(p => p.Acompanhantes)
                    .FirstOrDefaultAsync(p => p.Id == request.Id);

                if (participante == null)
                {
                    return Json(new { success = false, message = "Participante não encontrado" });
                }

                participante.Nome = request.Name;
                participante.Comparecimento = request.Attendance;

                // Remover acompanhantes existentes
                _context.Acompanhantes.RemoveRange(participante.Acompanhantes);

                // Adicionar novos acompanhantes
                if (request.Companions != null)
                {
                    foreach (var companionName in request.Companions.Where(c => !string.IsNullOrWhiteSpace(c)))
                    {
                        participante.Acompanhantes.Add(new Acompanhante
                        {
                            Nome = companionName.Trim(),
                            ParticipanteId = participante.Id
                        });
                    }
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

        [HttpGet]
        public async Task<IActionResult> ExportExcel()
        {
            var participantes = await _context.Participantes
                .Include(p => p.Acompanhantes)
                .OrderByDescending(p => p.DataCriacao)
                .ToListAsync();

            // Configurar EPPlus para uso não comercial
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Participantes Family Day");

            // Configurar título principal
            var titleRange = worksheet.Cells[1, 1, 1, 6];
            titleRange.Merge = true;
            titleRange.Value = "IBSVF FAMILY DAY - LISTA DE PARTICIPANTES";
            titleRange.Style.Font.Size = 18;
            titleRange.Style.Font.Bold = true;
            titleRange.Style.Font.Color.SetColor(Color.White);
            titleRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
            titleRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(74, 111, 165)); // Azul principal
            titleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            titleRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            titleRange.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.FromArgb(74, 111, 165));
            worksheet.Row(1).Height = 35;

            // Configurar subtítulo com data
            var subtitleRange = worksheet.Cells[2, 1, 2, 6];
            subtitleRange.Merge = true;
            subtitleRange.Value = $"Relatório Gerado em: {DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy 'às' HH:mm", new System.Globalization.CultureInfo("pt-BR"))}";
            subtitleRange.Style.Font.Size = 12;
            subtitleRange.Style.Font.Italic = true;
            subtitleRange.Style.Font.Color.SetColor(Color.FromArgb(74, 111, 165));
            subtitleRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            subtitleRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.Row(2).Height = 25;

            // Configurar cabeçalho da tabela
            var headers = new string[] { "Nº", "Nome Completo", "Status", "Acompanhantes", "Total de Pessoas", "Data de Cadastro" };
            
            // Aplicar estilo ao cabeçalho
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[4, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.Size = 11;
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(52, 73, 94)); // Azul escuro
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                cell.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.White);
            }
            worksheet.Row(4).Height = 30;

            // Adicionar dados
            int row = 5;
            int confirmados = 0;
            int totalPessoas = 0;
            int contador = 1;

            foreach (var participante in participantes)
            {
                // Número sequencial
                var numeroCell = worksheet.Cells[row, 1];
                numeroCell.Value = contador;
                numeroCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                numeroCell.Style.Font.Bold = true;
                numeroCell.Style.Font.Color.SetColor(Color.FromArgb(52, 73, 94));

                // Nome
                var nomeCell = worksheet.Cells[row, 2];
                nomeCell.Value = participante.Nome;
                nomeCell.Style.Font.Size = 10;
                nomeCell.Style.Font.Bold = true;
                
                // Status com cores diferenciadas
                var statusCell = worksheet.Cells[row, 3];
                var status = participante.Comparecimento == "yes" ? "✓ CONFIRMADO" : "✗ NÃO CONFIRMADO";
                statusCell.Value = status;
                statusCell.Style.Font.Bold = true;
                statusCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                statusCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                
                if (participante.Comparecimento == "yes")
                {
                    statusCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    statusCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(212, 237, 218)); // Verde claro
                    statusCell.Style.Font.Color.SetColor(Color.FromArgb(25, 135, 84)); // Verde escuro
                    statusCell.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.FromArgb(25, 135, 84));
                    confirmados++;
                }
                else
                {
                    statusCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    statusCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(248, 215, 218)); // Vermelho claro
                    statusCell.Style.Font.Color.SetColor(Color.FromArgb(220, 53, 69)); // Vermelho escuro
                    statusCell.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.FromArgb(220, 53, 69));
                }

                // Acompanhantes com formatação especial
                var acompanhantesCell = worksheet.Cells[row, 4];
                if (participante.Acompanhantes.Any())
                {
                    var acompanhantesStr = string.Join("\n• ", participante.Acompanhantes.Select(a => a.Nome));
                    acompanhantesCell.Value = "• " + acompanhantesStr;
                    acompanhantesCell.Style.WrapText = true;
                    acompanhantesCell.Style.Font.Size = 9;
                }
                else
                {
                    acompanhantesCell.Value = "Nenhum acompanhante";
                    acompanhantesCell.Style.Font.Italic = true;
                    acompanhantesCell.Style.Font.Color.SetColor(Color.Gray);
                }
                acompanhantesCell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;

                // Total de pessoas com destaque
                var totalPessoasFamilia = 1 + participante.Acompanhantes.Count;
                var totalCell = worksheet.Cells[row, 5];
                totalCell.Value = totalPessoasFamilia;
                totalCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                totalCell.Style.Font.Bold = true;
                totalCell.Style.Font.Size = 12;
                
                // Cor baseada na quantidade
                if (totalPessoasFamilia > 1)
                {
                    totalCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    totalCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 243, 205)); // Amarelo claro
                    totalCell.Style.Font.Color.SetColor(Color.FromArgb(133, 100, 4)); // Amarelo escuro
                }
                
                if (participante.Comparecimento == "yes")
                {
                    totalPessoas += totalPessoasFamilia;
                }

                // Data formatada
                var dataCell = worksheet.Cells[row, 6];
                dataCell.Value = participante.DataCriacao.ToString("dd/MM/yyyy\nHH:mm");
                dataCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                dataCell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                dataCell.Style.Font.Size = 9;
                dataCell.Style.WrapText = true;

                // Aplicar bordas e alternância de cores nas linhas
                for (int col = 1; col <= headers.Length; col++)
                {
                    var cell = worksheet.Cells[row, col];
                    cell.Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.LightGray);
                    
                    // Zebra striping mais suave
                    if (row % 2 == 0 && col != 3) // Não aplicar zebra na coluna de status
                    {
                        if (cell.Style.Fill.PatternType != ExcelFillStyle.Solid)
                        {
                            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(248, 249, 250));
                        }
                    }
                }

                // Altura dinâmica baseada nos acompanhantes
                var altura = participante.Acompanhantes.Count > 0 ? Math.Max(25, participante.Acompanhantes.Count * 15 + 10) : 25;
                worksheet.Row(row).Height = altura;

                row++;
                contador++;
            }

            // Adicionar resumo estatístico com visual aprimorado
            row += 2;
            var summaryStartRow = row;
            
            // Título do resumo com gradiente
            var summaryTitle = worksheet.Cells[summaryStartRow, 1, summaryStartRow, headers.Length];
            summaryTitle.Merge = true;
            summaryTitle.Value = "📊 RESUMO ESTATÍSTICO DO EVENTO";
            summaryTitle.Style.Font.Bold = true;
            summaryTitle.Style.Font.Size = 16;
            summaryTitle.Style.Font.Color.SetColor(Color.White);
            summaryTitle.Style.Fill.PatternType = ExcelFillStyle.Solid;
            summaryTitle.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(52, 73, 94));
            summaryTitle.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            summaryTitle.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            summaryTitle.Style.Border.BorderAround(ExcelBorderStyle.Medium, Color.FromArgb(52, 73, 94));
            worksheet.Row(summaryStartRow).Height = 35;

            // Estatísticas com cards visuais
            summaryStartRow += 2;
            var stats = new[]
            {
                new { Label = "👥 Total de Famílias Cadastradas", Value = participantes.Count.ToString(), Color = Color.FromArgb(74, 111, 165) },
                new { Label = "✅ Famílias Confirmadas", Value = confirmados.ToString(), Color = Color.FromArgb(25, 135, 84) },
                new { Label = "❌ Famílias Não Confirmadas", Value = (participantes.Count - confirmados).ToString(), Color = Color.FromArgb(220, 53, 69) },
                new { Label = "🎉 Total de Pessoas Confirmadas", Value = totalPessoas.ToString(), Color = Color.FromArgb(255, 193, 7) },
                new { Label = "📅 Data da Exportação", Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm"), Color = Color.FromArgb(108, 117, 125) }
            };

            foreach (var stat in stats)
            {
                // Label com ícone
                var labelCell = worksheet.Cells[summaryStartRow, 2];
                labelCell.Value = stat.Label + ":";
                labelCell.Style.Font.Bold = true;
                labelCell.Style.Font.Size = 11;
                labelCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                // Valor com destaque
                var valueCell = worksheet.Cells[summaryStartRow, 4];
                valueCell.Value = stat.Value;
                valueCell.Style.Font.Bold = true;
                valueCell.Style.Font.Size = 12;
                valueCell.Style.Font.Color.SetColor(stat.Color);
                valueCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                valueCell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(Math.Min(255, stat.Color.R + 40), Math.Min(255, stat.Color.G + 40), Math.Min(255, stat.Color.B + 40)));
                valueCell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                valueCell.Style.Border.BorderAround(ExcelBorderStyle.Medium, stat.Color);

                worksheet.Row(summaryStartRow).Height = 25;
                summaryStartRow++;
            }

            // Ajustar largura das colunas
            worksheet.Column(1).Width = 8;   // Nº
            worksheet.Column(2).Width = 30;  // Nome
            worksheet.Column(3).Width = 18;  // Status
            worksheet.Column(4).Width = 35;  // Acompanhantes
            worksheet.Column(5).Width = 15;  // Total de Pessoas
            worksheet.Column(6).Width = 15;  // Data

            // Congelar linhas de título e cabeçalho
            worksheet.View.FreezePanes(5, 1);

            // Aplicar filtro automático
            worksheet.Cells[4, 1, participantes.Count + 4, headers.Length].AutoFilter = true;

            // Adicionar rodapé
            var footerRow = summaryStartRow + 2;
            var footerRange = worksheet.Cells[footerRow, 1, footerRow, headers.Length];
            footerRange.Merge = true;
            footerRange.Value = "Relatório gerado automaticamente pelo Sistema IBSVF Family Day";
            footerRange.Style.Font.Size = 9;
            footerRange.Style.Font.Italic = true;
            footerRange.Style.Font.Color.SetColor(Color.Gray);
            footerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            // Nome do arquivo mais descritivo
            var fileName = $"IBSVF_Family_Day_Participantes_{DateTime.Now:dd-MM-yyyy_HH-mm}.xlsx";
            var fileBytes = package.GetAsByteArray();

            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
