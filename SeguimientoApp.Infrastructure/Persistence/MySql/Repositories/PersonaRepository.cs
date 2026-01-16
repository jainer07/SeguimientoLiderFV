using Microsoft.EntityFrameworkCore;
using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;
using SeguimientoApp.Infrastructure.Persistence.MySql.Models;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Repositories
{
    public class PersonaRepository(AppDbContext db) : IPersonaRepositoryPort
    {
        private readonly AppDbContext _db = db;

        public async Task<List<PersonaDto>> GetAllAsync(CancellationToken ct = default)
        {
            return await _db.PersonaModels
               .AsNoTracking()
               .Include(p => p.CatalogoTipoDocumento)
               .OrderBy(p => p.NumeroDocumento)
               .Select(p => new PersonaDto
               {
                   IdPersona = p.IdPersona,
                   TipoDocumentoCodigo = p.CatalogoTipoDocumento.Codigo,
                   NumeroDocumento = p.NumeroDocumento,
                   PrimerNombre = p.PrimerNombre,
                   SegundoNombre = p.SegundoNombre,
                   PrimerApellido = p.PrimerApellido,
                   SegundoApellido = p.SegundoApellido,
                   Celular = p.Celular,
                   Correo = p.Correo,
                   Direccion = p.Direccion,
                   EsLider = p.EsLider,
                   Estado = p.Estado,
                   LugarVotacion = p.LugarVotacion,
                   Mesa = p.Mesa,
               })
               .ToListAsync(ct);
        }

        public async Task<PersonaEditDto?> GetByIdAsync(long idPersona, CancellationToken ct = default)
        {
            return await _db.PersonaModels
                .AsNoTracking()
                .Include(p => p.CatalogoTipoDocumento)
                .Where(p => p.IdPersona == idPersona)
                .Select(p => new PersonaEditDto
                {
                    IdPersona = p.IdPersona,
                    TipoDocumentoCodigo = p.CatalogoTipoDocumento.Codigo,
                    NumeroDocumento = p.NumeroDocumento,

                    PrimerNombre = p.PrimerNombre,
                    SegundoNombre = p.SegundoNombre,
                    PrimerApellido = p.PrimerApellido,
                    SegundoApellido = p.SegundoApellido,
                    Celular = p.Celular,
                    Correo = p.Correo,
                    Direccion = p.Direccion,
                    EsLider = p.EsLider,
                    Estado = p.Estado,
                    LugarVotacion = p.LugarVotacion,
                    Mesa = p.Mesa,
                    MesaNumero = TryParseMesa(p.Mesa)
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> ExistsByDocumentoAsync(int tipoDocumento, long numeroDocumento, CancellationToken ct = default)
        {
            return await _db.PersonaModels
                .AsNoTracking()
                .AnyAsync(p => p.IdTipoDocumento == tipoDocumento && p.NumeroDocumento == numeroDocumento, ct);
        }

        public async Task ToggleEstadoAsync(long idPersona, CancellationToken ct = default)
        {
            var persona = await _db.PersonaModels.FirstOrDefaultAsync(p => p.IdPersona == idPersona, ct);
            if (persona == null) return;

            persona.Estado = !persona.Estado;
            await _db.SaveChangesAsync(ct);
        }

        public async Task CreateAsync(PersonaCreateDto dto, CancellationToken ct = default)
        {
            var persona = new PersonaModel
            {
                IdTipoDocumento = dto.IdTipoDocumento ?? 0,
                NumeroDocumento = dto.NumeroDocumento,
                PrimerNombre = dto.PrimerNombre.Trim(),
                SegundoNombre = dto.SegundoNombre?.Trim() ?? "",
                PrimerApellido = dto.PrimerApellido.Trim(),
                SegundoApellido = dto.SegundoApellido?.Trim() ?? "",
                Celular = dto.Celular ?? 0,
                Correo = dto.Correo?.Trim() ?? "",
                Direccion = dto.Direccion?.Trim() ?? "",
                EsLider = dto.EsLider,
                Estado = dto.Estado,
                LugarVotacion = dto.LugarVotacion?.Trim() ?? "",
                Mesa = dto.MesaNumero.HasValue ? $"Mesa {dto.MesaNumero.Value}" : ""
            };

            _db.PersonaModels.Add(persona);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(PersonaEditDto dto, CancellationToken ct = default)
        {
            var persona = await _db.PersonaModels
                .FirstOrDefaultAsync(p => p.IdPersona == dto.IdPersona, ct);

            if (persona == null) return;

            persona.PrimerNombre = dto.PrimerNombre?.Trim() ?? "";
            persona.SegundoNombre = dto.SegundoNombre?.Trim() ?? "";
            persona.PrimerApellido = dto.PrimerApellido?.Trim() ?? "";
            persona.SegundoApellido = dto.SegundoApellido?.Trim() ?? "";
            persona.Celular = dto.Celular ?? 0;
            persona.Correo = dto.Correo?.Trim() ?? "";
            persona.Direccion = dto.Direccion?.Trim() ?? "";
            persona.EsLider = dto.EsLider;
            persona.Estado = dto.Estado;
            persona.LugarVotacion = dto.LugarVotacion?.Trim() ?? "";
            persona.Mesa = dto.MesaNumero.HasValue ? $"Mesa {dto.MesaNumero.Value}" : "";

            await _db.SaveChangesAsync(ct);
        }


        private static int? TryParseMesa(string? mesa)
        {
            if (string.IsNullOrWhiteSpace(mesa)) return null;

            var s = mesa.Trim();
            if (s.StartsWith("Mesa", StringComparison.OrdinalIgnoreCase))
                s = s.Substring(4).Trim();

            return int.TryParse(s, out var n) ? n : null;
        }
    }
}
