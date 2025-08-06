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

            // Configuração da tabela Participante
            modelBuilder.Entity<Participante>(entity =>
            {
                entity.Property(e => e.DataCriacao)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                entity.HasCheckConstraint("CK_participantes_comparecimento", 
                    "comparecimento IN ('yes', 'no')");
                
                entity.HasIndex(e => e.Nome, "IX_participantes_nome");
                entity.HasIndex(e => e.Comparecimento, "IX_participantes_comparecimento");
            });

            // Configuração da tabela Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                entity.HasIndex(e => e.Username, "IX_usuarios_username")
                    .IsUnique();
            });

            // Configuração da tabela Acompanhante
            modelBuilder.Entity<Acompanhante>(entity =>
            {
                entity.HasIndex(e => e.ParticipanteId, "IX_acompanhantes_participante_id");
            });

            // Configurações de relacionamento
            modelBuilder.Entity<Participante>()
                .HasMany(p => p.Acompanhantes)
                .WithOne(a => a.Participante)
                .HasForeignKey(a => a.ParticipanteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed do usuário admin
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
