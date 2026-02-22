using Microsoft.EntityFrameworkCore;
using SeguimientoApp.Infrastructure.Persistence.MySql.Models;

namespace SeguimientoApp.Infrastructure.Persistence.MySql
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatalogoModel>().HasOne(c => c.TipoCatalogo)
                                                .WithMany(tc => tc.Catalogos)
                                                .HasForeignKey(c => c.IdTipoCatalogo);

            modelBuilder.Entity<PersonaModel>().HasOne(p => p.CatalogoTipoDocumento)
                                               .WithMany(c => c.Personas)
                                               .HasForeignKey(p => p.IdTipoDocumento);

            modelBuilder.Entity<PersonaLiderModel>().HasKey(x => new { x.IdLider, x.IdPersona });

            modelBuilder.Entity<PersonaLiderModel>().HasOne(x => x.Lider)
                                                    .WithMany()
                                                    .HasForeignKey(x => x.IdLider);

            modelBuilder.Entity<PersonaLiderModel>().HasOne(x => x.Persona)
                                                    .WithMany()
                                                    .HasForeignKey(x => x.IdPersona);

            modelBuilder.Entity<EventoModel>().HasOne(e => e.TipoEvento)
                                              .WithMany()
                                              .HasForeignKey(e => e.IdTipoEventoCat);

            modelBuilder.Entity<EventoModel>().HasOne(e => e.EstadoEvento)
                                              .WithMany()
                                              .HasForeignKey(e => e.IdEstadoEventoCat);

            modelBuilder.Entity<EventoActividadModel>().HasIndex(x => new { x.IdEvento, x.Orden })
                                                       .IsUnique();

            modelBuilder.Entity<ActividadRegistroModel>().HasIndex(x => new { x.IdEventoActividad, x.IdEventoParticipante })
                                                         .IsUnique();

            modelBuilder.Entity<SmsOutboxModel>()
                .HasOne(x => x.Job)
                .WithMany(j => j.Outbox)
                .HasForeignKey(x => x.IdJob);

            modelBuilder.Entity<SmsOutboxModel>()
                .HasIndex(x => new { x.Estado, x.NextAttemptAt });

            modelBuilder.Entity<SmsOutboxModel>()
                .HasIndex(x => x.IdJob);

        }

        public DbSet<CatalogoModel> CatalogoModels { get; private set; }
        public DbSet<ClienteModel> ClienteModels { get; private set; }
        public DbSet<PersonaModel> PersonaModels { get; private set; }
        public DbSet<PersonaLiderModel> PersonaLiderModels { get; private set; }
        public DbSet<EventoModel> EventoModels { get; set; }
        public DbSet<ActividadPlantillaModel> ActividadPlantillaModels { get; set; }
        public DbSet<EventoActividadModel> EventoActividadModels { get; set; }
        public DbSet<EventoParticipanteModel> EventoParticipanteModels { get; set; }
        public DbSet<ActividadRegistroModel> ActividadRegistroModels { get; set; }
        public DbSet<SmsJobModel> SmsJobModels { get; set; }
        public DbSet<SmsOutboxModel> SmsOutboxModels { get; set; }

    }
}
