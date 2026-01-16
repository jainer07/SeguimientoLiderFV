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
        }

        public DbSet<CatalogoModel> CatalogoModels { get; private set; }
        public DbSet<ClienteModel> ClienteModels { get; private set; }
        public DbSet<PersonaModel> PersonaModels { get; private set; }
    }
}
