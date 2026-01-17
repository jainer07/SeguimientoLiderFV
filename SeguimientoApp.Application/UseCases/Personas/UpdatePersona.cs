using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;
using SeguimientoApp.Domain.Enums;

namespace SeguimientoApp.Application.UseCases.Personas
{
    public class UpdatePersona(IPersonaRepositoryPort repo)
    {
        private readonly IPersonaRepositoryPort _repo = repo;

        public Task ExecuteAsync(PersonaEditDto dto, CancellationToken ct = default)
            => _repo.UpdateAsync(dto, ct);

        public Task ToggleEstadoAsync(long idPersona, CancellationToken ct = default)
             => _repo.ToggleEstadoAsync(idPersona, ct);

        public Task<PersonaLiderAssignResult> AddPersonaToLiderAsync(long idLider, long idPersona, CancellationToken ct = default)
             => _repo.AddPersonaToLiderAsync(idLider, idPersona, ct);

        public Task RemovePersonaFromLiderAsync(long idLider, long idPersona, CancellationToken ct = default)
             => _repo.RemovePersonaFromLiderAsync(idLider, idPersona, ct);
    }
}
