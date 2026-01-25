using SeguimientoApp.Application.DTOs;

namespace SeguimientoApp.Application.Ports.Persistence
{
    public interface IEventoRepositoryPort
    {
        Task<List<EventoDto>> GetAllAsync(CancellationToken ct);
        Task<EventoEditDto?> GetByIdAsync(int idEvento, CancellationToken ct);
        Task<int> CreateAsync(EventoCreateDto dto, CancellationToken ct);
        Task UpdateAsync(EventoEditDto dto, CancellationToken ct = default);
        Task UpdateEstadoAsync(int idEvento, int idEstadoEventoCat, CancellationToken ct);
    }
}
