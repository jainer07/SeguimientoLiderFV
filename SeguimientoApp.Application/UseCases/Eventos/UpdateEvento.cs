using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Application.UseCases.Eventos
{
    public class UpdateEvento(IEventoRepositoryPort repo)
    {
        private readonly IEventoRepositoryPort _repo = repo;

        public Task ExecuteAsync(EventoEditDto dto, CancellationToken ct = default)
            => _repo.UpdateAsync(dto, ct);

        public Task UpdateEstadoAsync(int idEvento, int idEstadoEventoCat, CancellationToken ct = default)
            => _repo.UpdateEstadoAsync(idEvento, idEstadoEventoCat, ct);
    }
}
