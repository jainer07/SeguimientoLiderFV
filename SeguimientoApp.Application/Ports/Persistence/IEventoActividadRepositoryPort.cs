using SeguimientoApp.Application.DTOs;

namespace SeguimientoApp.Application.Ports.Persistence
{
    public interface IEventoActividadRepositoryPort
    {
        Task SetActividadesAsync(int idEvento, List<EventoActividadDto> actividades, CancellationToken ct);
        Task<List<EventoActividadDto>> GetByEventoAsync(int idEvento, CancellationToken ct);
    }

}
