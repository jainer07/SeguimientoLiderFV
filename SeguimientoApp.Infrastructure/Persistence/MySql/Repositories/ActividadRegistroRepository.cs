using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Repositories
{
    public class ActividadRegistroRepository : IActividadRegistroRepositoryPort
    {
        public Task<bool> HasPendientesReglaCierreAsync(int idEvento, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task UpsertAsync(ActividadRegistroDto dto, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
