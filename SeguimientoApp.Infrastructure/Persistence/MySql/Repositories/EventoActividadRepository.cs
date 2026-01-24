using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Repositories
{
    public class EventoActividadRepository : IEventoActividadRepositoryPort
    {
        public Task<List<EventoActividadDto>> GetByEventoAsync(int idEvento, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task SetActividadesAsync(int idEvento, List<EventoActividadDto> actividades, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
