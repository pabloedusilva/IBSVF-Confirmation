using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBSVF.Web.Models
{
    [Table("participantes")]
    public class Participante
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("nome")]
        [MaxLength(255)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [Column("comparecimento")]
        [MaxLength(10)]
        public string Comparecimento { get; set; } = string.Empty;

        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Acompanhante> Acompanhantes { get; set; } = new List<Acompanhante>();
    }
}
