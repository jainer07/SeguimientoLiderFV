using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Application.UseCases.Personas
{
    public class GetPersonas(IPersonaRepositoryPort repo)
    {
        private readonly IPersonaRepositoryPort _repo = repo;

        public Task<List<PersonaDto>> ExecuteAsync(CancellationToken ct = default)
            => _repo.GetAllAsync(ct);
    }
}
