using Microsoft.EntityFrameworkCore;
using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;
using SeguimientoApp.Domain.Enums;
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

        public async Task<PersonaLookupResultDto> LookupByCedulaAsync(long idLider, long cedula, CancellationToken ct = default)
        {
            var persona = await _db.PersonaModels
                .AsNoTracking()
                .Include(p => p.CatalogoTipoDocumento)
                .Where(p => p.NumeroDocumento == cedula)
                .Select(p => new
                {
                    p.IdPersona,
                    TipoDocumentoCodigo = p.CatalogoTipoDocumento.Codigo,
                    p.NumeroDocumento,
                    p.PrimerNombre,
                    p.SegundoNombre,
                    p.PrimerApellido,
                    p.SegundoApellido,
                    p.Celular,
                    p.Estado,
                    p.EsLider
                })
                .FirstOrDefaultAsync(ct);

            if (persona == null)
                return new PersonaLookupResultDto { Code = "NOT_FOUND" };

            var personaMini = new PersonaMiniDto
            {
                IdPersona = persona.IdPersona,
                TipoDocumentoCodigo = persona.TipoDocumentoCodigo,
                NumeroDocumento = persona.NumeroDocumento,
                NombreCompleto = (persona.PrimerNombre + " " + persona.SegundoNombre + " " + persona.PrimerApellido + " " + persona.SegundoApellido).Trim(),
                Celular = persona.Celular,
                Estado = persona.Estado
            };

            if (persona.IdPersona == idLider)
                return new PersonaLookupResultDto { Code = "SELF", Persona = personaMini };

            if (!persona.Estado)
                return new PersonaLookupResultDto { Code = "INACTIVE", Persona = personaMini };

            if (persona.EsLider)
                return new PersonaLookupResultDto { Code = "IS_LIDER", Persona = personaMini };

            var rel = await _db.PersonaLiderModels
                .AsNoTracking()
                .Where(x => x.IdPersona == persona.IdPersona)
                .Include(x => x.Lider)
                    .ThenInclude(l => l.CatalogoTipoDocumento)
                .Select(x => new
                {
                    x.IdLider,
                    Lider = new PersonaMiniDto
                    {
                        IdPersona = x.Lider.IdPersona,
                        TipoDocumentoCodigo = x.Lider.CatalogoTipoDocumento.Codigo,
                        NumeroDocumento = x.Lider.NumeroDocumento,
                        NombreCompleto =
                            (x.Lider.PrimerNombre + " " + x.Lider.SegundoNombre + " " + x.Lider.PrimerApellido + " " + x.Lider.SegundoApellido).Trim(),
                        Celular = x.Lider.Celular,
                        Estado = x.Lider.Estado
                    }
                })
                .FirstOrDefaultAsync(ct);

            

            if (rel == null)
                return new PersonaLookupResultDto { Code = "AVAILABLE", Persona = personaMini };

            return new PersonaLookupResultDto
            {
                Code = "ASSIGNED",
                Persona = personaMini,
                LiderActual = rel.Lider
            };
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

        public async Task<PersonaDetailsDto?> GetDetailsAsync(long idPersona, CancellationToken ct = default)
        {
            var persona = await GetByIdAsync(idPersona, ct);
            if (persona == null) return null;

            var personasACargo = await _db.PersonaLiderModels
                .AsNoTracking()
                .Where(x => x.IdLider == idPersona)
                .Include(x => x.Persona)
                .ThenInclude(p => p.CatalogoTipoDocumento)
                .Select(x => new PersonaMiniDto
                {
                    IdPersona = x.Persona.IdPersona,
                    TipoDocumentoCodigo = x.Persona.CatalogoTipoDocumento.Codigo,
                    NumeroDocumento = x.Persona.NumeroDocumento,
                    NombreCompleto =
                        $"{x.Persona.PrimerNombre} {x.Persona.SegundoNombre} {x.Persona.PrimerApellido} {x.Persona.SegundoApellido}".Trim(),
                    Celular = x.Persona.Celular,
                    Estado = x.Persona.Estado
                })
                .ToListAsync(ct);

            var candidatos = await _db.PersonaModels
                .AsNoTracking()
                .Include(p => p.CatalogoTipoDocumento)
                .Where(p =>
                    p.IdPersona != idPersona &&
                    !_db.PersonaLiderModels.Any(pl => pl.IdPersona == p.IdPersona))
                .OrderBy(p => p.PrimerNombre)
                .ThenBy(p => p.SegundoNombre)
                .ThenBy(p => p.PrimerApellido)
                .ThenBy(p => p.SegundoApellido)
                .Select(p => new PersonaMiniDto
                {
                    IdPersona = p.IdPersona,
                    TipoDocumentoCodigo = p.CatalogoTipoDocumento.Codigo,
                    NumeroDocumento = p.NumeroDocumento,
                    NombreCompleto =
                        $"{p.PrimerNombre} {p.SegundoNombre} {p.PrimerApellido} {p.SegundoApellido}".Trim(),
                    Celular = p.Celular,
                    Estado = p.Estado
                })
                .ToListAsync(ct);

            return new PersonaDetailsDto
            {
                Persona = persona,
                PersonasACargo = personasACargo,
                Candidatos = candidatos
            };
        }

        public async Task<PersonaLiderAssignResult> AddPersonaToLiderAsync(long idLider, long idPersona, CancellationToken ct = default)
        {
            if (idLider == idPersona) 
                return PersonaLiderAssignResult.SamePerson;

            var persona = await _db.PersonaModels
                .AsNoTracking()
                .Where(p => p.IdPersona == idPersona)
                .Select(p => new
                {
                    p.IdPersona,
                    p.Estado,
                    p.EsLider
                })
                .FirstOrDefaultAsync(ct);

            if (persona == null)
                return PersonaLiderAssignResult.PersonaNotFound;

            if (!persona.Estado)
                return PersonaLiderAssignResult.PersonaInactive;

            if (persona.EsLider)
                return PersonaLiderAssignResult.PersonaIsLider;

            var alreadyAssigned = await _db.PersonaLiderModels.AnyAsync(x => x.IdPersona == idPersona, ct);
            if (alreadyAssigned) 
                return PersonaLiderAssignResult.AlreadyAssigned;

            var existsSame = await _db.PersonaLiderModels.AnyAsync(x => x.IdLider == idLider && x.IdPersona == idPersona, ct);
            if (existsSame)
                return PersonaLiderAssignResult.DuplicateRelation;

            _db.PersonaLiderModels.Add(new PersonaLiderModel
            {
                IdLider = idLider,
                IdPersona = idPersona
            });

            await _db.SaveChangesAsync(ct);
            return PersonaLiderAssignResult.Ok;
        }

        public async Task RemovePersonaFromLiderAsync(long idLider, long idPersona, CancellationToken ct = default)
        {
            var rel = await _db.PersonaLiderModels.FirstOrDefaultAsync(x => x.IdLider == idLider && x.IdPersona == idPersona, ct);

            if (rel == null) return;

            _db.PersonaLiderModels.Remove(rel);
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
