using System.ComponentModel.DataAnnotations;

namespace IBSVF.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O usuário é obrigatório")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Password { get; set; } = string.Empty;
    }

    public class ParticipanteViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "A confirmação de presença é obrigatória")]
        public string Attendance { get; set; } = string.Empty;

        public string HasCompanions { get; set; } = "no";
        
        public List<string> Companions { get; set; } = new List<string>();
    }

    public class DashboardViewModel
    {
        public int ConfirmedCount { get; set; }
        public int NotConfirmedCount { get; set; }
        public int TotalPeople { get; set; }
        public int FamiliesCount { get; set; }
        public List<ParticipanteDetalhado> Participantes { get; set; } = new List<ParticipanteDetalhado>();
    }

    public class ParticipanteDetalhado
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Comparecimento { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public List<string> Acompanhantes { get; set; } = new List<string>();
        public string Status => Comparecimento == "yes" ? "Confirmado" : "Não Confirmado";
        public int TotalPessoas => 1 + Acompanhantes.Count;
    }

    public class EditParticipanteViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A confirmação de presença é obrigatória")]
        public string Comparecimento { get; set; } = string.Empty;
        
        public string HasCompanions { get; set; } = "no";
        
        public List<string> Acompanhantes { get; set; } = new List<string>();
    }

    public class UpdateParticipantRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Attendance { get; set; } = string.Empty;
        public List<string> Companions { get; set; } = new List<string>();
    }
}
