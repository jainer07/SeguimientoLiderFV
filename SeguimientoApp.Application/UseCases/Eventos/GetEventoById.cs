using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Application.UseCases.Eventos
{
    public class GetEventoById(IEventoRepositoryPort repo)
    {
        private readonly IEventoRepositoryPort _repo = repo;

        public Task<EventoEditDto?> ExecuteAsync(int idEvento, CancellationToken ct = default)
            => _repo.GetByIdAsync(idEvento, ct);
    }
}
