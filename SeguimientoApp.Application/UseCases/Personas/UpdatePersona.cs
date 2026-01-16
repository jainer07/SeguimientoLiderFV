using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Application.UseCases.Personas
{
    public class UpdatePersona(IPersonaRepositoryPort repo)
    {
        private readonly IPersonaRepositoryPort _repo = repo;

        public Task ExecuteAsync(PersonaEditDto dto, CancellationToken ct = default)
            => _repo.UpdateAsync(dto, ct);

        public Task ToggleEstadoAsync(long idPersona, CancellationToken ct = default)
             => _repo.ToggleEstadoAsync(idPersona, ct);
    }
}
