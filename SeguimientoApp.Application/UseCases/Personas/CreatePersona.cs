using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Application.UseCases.Personas
{
    public class CreatePersona(IPersonaRepositoryPort repo)
    {
        private readonly IPersonaRepositoryPort _repo = repo;

        public Task ExecuteAsync(PersonaCreateDto dto, CancellationToken ct = default)
            =>  _repo.CreateAsync(dto, ct);

        public Task<bool> ExistsByDocumentoAsync(int tipoDocumento, long numeroDocumento, CancellationToken ct = default)
            => _repo.ExistsByDocumentoAsync(tipoDocumento, numeroDocumento, ct);
    }
}
