using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBSVF.Web.Models
{
    [Table("acompanhantes")]
    public class Acompanhante
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("participante_id")]
        public int ParticipanteId { get; set; }

        [Required]
        [Column("nome")]
        [MaxLength(255)]
        public string Nome { get; set; } = string.Empty;

        [ForeignKey("ParticipanteId")]
        public virtual Participante Participante { get; set; } = null!;
    }
}
