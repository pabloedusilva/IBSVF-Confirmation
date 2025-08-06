using Microsoft.EntityFrameworkCore;
using IBSVF.Web.Models;

namespace IBSVF.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Participante> Participantes { get; set; }
        public DbSet<Acompanhante> Acompanhantes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações adicionais se necessário
            modelBuilder.Entity<Participante>()
                .HasMany(p => p.Acompanhantes)
                .WithOne(a => a.Participante)
                .HasForeignKey(a => a.ParticipanteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
