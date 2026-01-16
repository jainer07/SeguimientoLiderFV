using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Application.UseCases.Personas
{
    public class GetPersonaById(IPersonaRepositoryPort repo)
    {
        private readonly IPersonaRepositoryPort _repo = repo;

        public Task<PersonaEditDto?> ExecuteAsync(long idPersona, CancellationToken ct = default)
            => _repo.GetByIdAsync(idPersona, ct);
    }
}
