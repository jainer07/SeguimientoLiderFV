using SeguimientoApp.Application.DTOs;

namespace SeguimientoApp.Application.Ports.Persistence
{
    public interface IActividadRegistroRepositoryPort
    {
        Task UpsertAsync(ActividadRegistroDto dto, CancellationToken ct);
        Task<bool> HasPendientesReglaCierreAsync(int idEvento, CancellationToken ct);
    }

}
