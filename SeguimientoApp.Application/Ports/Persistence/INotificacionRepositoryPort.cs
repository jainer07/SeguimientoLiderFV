using SeguimientoApp.Application.DTOs;

namespace SeguimientoApp.Application.Ports.Persistence
{
    public interface INotificacionRepositoryPort
    {
        Task<SmsSendResultDto> SendSmsAsync(string phone, string message, CancellationToken ct = default);
    }
}
