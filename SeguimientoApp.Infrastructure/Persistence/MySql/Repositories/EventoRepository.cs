using Microsoft.EntityFrameworkCore;
using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;
using SeguimientoApp.Infrastructure.Persistence.MySql.Models;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Repositories
{
    public class EventoRepository(AppDbContext db) : IEventoRepositoryPort
    {
        private readonly AppDbContext _db = db;

        public async Task<List<EventoDto>> GetAllAsync(CancellationToken ct)
        {
            return await _db.EventoModels
                .AsNoTracking()
                .Include(e => e.TipoEvento)
                .Include(e => e.EstadoEvento)
                .OrderByDescending(e => e.FechaInicio)
                .ThenByDescending(e => e.IdEvento)
                .Select(e => new EventoDto
                {
                    IdEvento = e.IdEvento,
                    Nombre = e.Nombre,

                    IdTipoEventoCat = e.IdTipoEventoCat,
                    TipoEventoNombre = e.TipoEvento.Nombre,
                    TipoEventoCodigo = e.TipoEvento.Codigo,

                    IdEstadoEventoCat = e.IdEstadoEventoCat,
                    EstadoEventoNombre = e.EstadoEvento.Nombre,
                    EstadoEventoCodigo = e.EstadoEvento.Codigo,

                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,

                    Lugar = e.Lugar ?? "",
                    Observaciones = e.Observaciones ?? ""
                })
                .ToListAsync(ct);
        }

        public async Task<EventoEditDto?> GetByIdAsync(int idEvento, CancellationToken ct)
        {
            return await _db.EventoModels
                .AsNoTracking()
                .Where(e => e.IdEvento == idEvento)
                .Select(e => new EventoEditDto
                {
                    IdEvento = e.IdEvento,
                    Nombre = e.Nombre,
                    IdTipoEventoCat = e.IdTipoEventoCat,
                    IdEstadoEventoCat = e.IdEstadoEventoCat,
                    FechaInicio = e.FechaInicio,
                    FechaFin = e.FechaFin,
                    Lugar = e.Lugar,
                    Observaciones = e.Observaciones
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<int> CreateAsync(EventoCreateDto dto, CancellationToken ct)
        {
            var model = new EventoModel
            {
                Nombre = dto.Nombre.Trim(),
                IdTipoEventoCat = dto.IdTipoEventoCat,
                IdEstadoEventoCat = dto.IdEstadoEventoCat,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                Lugar = dto.Lugar?.Trim() ?? "",
                Observaciones = dto.Observaciones?.Trim() ?? ""
            };

            _db.EventoModels.Add(model);
            await _db.SaveChangesAsync(ct);
            return model.IdEvento;
        }

        public async Task UpdateAsync(EventoEditDto dto, CancellationToken ct = default)
        {
            var model = await _db.EventoModels
               .FirstOrDefaultAsync(e => e.IdEvento == dto.IdEvento, ct);

            if (model == null) return;

            model.Nombre = dto.Nombre.Trim();
            model.IdTipoEventoCat = dto.IdTipoEventoCat;
            model.IdEstadoEventoCat = dto.IdEstadoEventoCat;
            model.FechaInicio = dto.FechaInicio;
            model.FechaFin = dto.FechaFin;
            model.Lugar = dto.Lugar?.Trim() ?? "";
            model.Observaciones = dto.Observaciones?.Trim() ?? "";

            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateEstadoAsync(int idEvento, int idEstadoEventoCat, CancellationToken ct)
        {
            var model = await _db.EventoModels
               .FirstOrDefaultAsync(e => e.IdEvento == idEvento, ct);

            if (model == null) return;

            model.IdEstadoEventoCat = idEstadoEventoCat;
            await _db.SaveChangesAsync(ct);
        }
    }
}
