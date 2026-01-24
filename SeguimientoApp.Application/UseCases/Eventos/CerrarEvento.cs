using SeguimientoApp.Application.Ports.Persistence;
using SeguimientoApp.Domain.Enums;

namespace SeguimientoApp.Application.UseCases.Eventos
{
    public class CerrarEvento(IEventoRepositoryPort eventoRepo, IActividadRegistroRepositoryPort registroRepo)
    {
        private readonly IEventoRepositoryPort _eventoRepo = eventoRepo;
        private readonly IActividadRegistroRepositoryPort _registroRepo = registroRepo;

        public async Task<bool> ExecuteAsync(int idEvento, CancellationToken ct)
        {
            var pendientes = await _registroRepo.HasPendientesReglaCierreAsync(idEvento, ct);
            if (pendientes)
                return false;

            await _eventoRepo.UpdateEstadoAsync(idEvento, (int)EstadoEvento.Cerrado, ct);
            return true;
        }
    }
}
