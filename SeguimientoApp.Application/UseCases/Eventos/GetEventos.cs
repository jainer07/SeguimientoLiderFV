using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Application.UseCases.Eventos
{
    public class GetEventos(IEventoRepositoryPort repo)
    {
        private readonly IEventoRepositoryPort _repo = repo;

        public Task<List<EventoDto>> ExecuteAsync(CancellationToken ct = default)
            => _repo.GetAllAsync(ct);
    }
}
