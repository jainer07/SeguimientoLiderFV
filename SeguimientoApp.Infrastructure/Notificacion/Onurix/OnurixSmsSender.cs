using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;
using SeguimientoApp.Infrastructure.Notificacion.Onurix.Models;
using System.Text.Json;

namespace SeguimientoApp.Infrastructure.Notificacion.Onurix
{
    public class OnurixSmsSender(HttpClient http, OnurixOptions opt) : INotificacionRepositoryPort
    {
        private readonly HttpClient _http = http;
        private readonly OnurixOptions _opt = opt;

        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

        public async Task<SmsSendResultDto> SendSmsAsync(string phone, string message, CancellationToken ct = default)
        {
            if (_opt.Client <= 0)
            {
                return new SmsSendResultDto()
                {
                    Ok = false,
                    ProviderMessageId = null,
                    ProviderState = null,
                    ErrorMessage = "Onurix.Client no configurado."
                };
            }
            if (string.IsNullOrWhiteSpace(_opt.Key))
            {
                return new SmsSendResultDto()
                {
                    Ok = false,
                    ProviderMessageId = null,
                    ProviderState = null,
                    ErrorMessage = "Onurix.Key no configurado."
                };
            }

            // Onurix usa x-www-form-urlencoded
            var form = new Dictionary<string, string>
            {
                ["client"] = _opt.Client.ToString(),
                ["key"] = _opt.Key,
                ["phone"] = phone,
                ["sms"] = message
            };

            using var content = new FormUrlEncodedContent(form);
            using var resp = await _http.PostAsync("/api/v1/sms/send", content, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            OnurixSendSmsResponse? parsed = null;
            try { parsed = JsonSerializer.Deserialize<OnurixSendSmsResponse>(body, JsonOpts); }
            catch { /* si no parsea, seguimos */ }

            if (!resp.IsSuccessStatusCode)
            {
                var msg = parsed?.msg ?? body;
                return new SmsSendResultDto()
                {
                    Ok = false,
                    ProviderMessageId = null,
                    ProviderState = null,
                    ErrorMessage = $"HTTP {(int)resp.StatusCode}: {msg}"
                };
            }

            if (parsed?.error is not null && parsed.error != 0)
            {
                return new SmsSendResultDto()
                {
                    Ok = false,
                    ProviderMessageId = null,
                    ProviderState = null,
                    ErrorMessage = parsed.msg ?? "Error Onurix"
                };
            }

            return new SmsSendResultDto()
            {
                Ok = true,
                ProviderMessageId = parsed?.data?.id ?? parsed?.id,
                ProviderState = parsed?.data?.state,
                ErrorMessage = null
            };
        }
    }
}
