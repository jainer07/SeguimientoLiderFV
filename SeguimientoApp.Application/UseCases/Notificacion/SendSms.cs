using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Application.UseCases.Notificacion
{
    public class SendSms(INotificacionRepositoryPort repo)
    {
        private readonly INotificacionRepositoryPort _repo = repo;

        public Task<SmsSendResultDto> ExecuteAsync(string phone, string message, CancellationToken ct = default) 
            => _repo.SendSmsAsync(phone, message, ct);
    }
}
