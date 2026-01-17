using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Application.UseCases.Personas
{
    public class GetPersonaById(IPersonaRepositoryPort repo)
    {
        private readonly IPersonaRepositoryPort _repo = repo;

        public Task<PersonaEditDto?> ExecuteAsync(long idPersona, CancellationToken ct = default)
            => _repo.GetByIdAsync(idPersona, ct);

        public Task<PersonaDetailsDto?> GetDetailsAsync(long idPersona, CancellationToken ct = default)
            => _repo.GetDetailsAsync(idPersona, ct);

        public Task<PersonaLookupResultDto> LookupByCedulaAsync(long idLider, long Cedula, CancellationToken ct = default)
            => _repo.LookupByCedulaAsync(idLider, Cedula, ct);
    }
}
