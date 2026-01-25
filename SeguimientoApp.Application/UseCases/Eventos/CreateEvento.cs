using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Application.UseCases.Eventos
{
    public class CreateEvento(IEventoRepositoryPort repo)
    {
        private readonly IEventoRepositoryPort _repo = repo;

        public Task<int> ExecuteAsync(EventoCreateDto dto, CancellationToken ct = default)
            => _repo.CreateAsync(dto, ct);
    }
}
