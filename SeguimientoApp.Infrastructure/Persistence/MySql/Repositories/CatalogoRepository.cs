using Microsoft.EntityFrameworkCore;
using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Repositories
{
    public class CatalogoRepository(AppDbContext db) : ICatalogoRepositoryPort
    {
        private readonly AppDbContext _db = db;

        public async Task<List<CatalogoItemDto>> GetLsCatalogoAsync(int tipoCatalogo, CancellationToken ct = default)
        {
            return await _db.CatalogoModels
                .AsNoTracking()
                .Where(c => c.IdTipoCatalogo == tipoCatalogo)
                .OrderBy(c => c.Nombre)
                .Select(c => new CatalogoItemDto
                {
                    IdCatalogo = c.IdCatalogo,
                    Nombre = c.Nombre,
                    Codigo = c.Codigo,
                    Valor = c.Valor,
                    Estado = c.Estado,
                })
                .ToListAsync(ct);
        }
    }
}
