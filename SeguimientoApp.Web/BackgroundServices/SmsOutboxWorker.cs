using Microsoft.Extensions.Options;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Web.BackgroundServices
{
    public class SmsOutboxWorker(IServiceScopeFactory scopeFactory, IOptions<SmsSendingOptions> options, ILogger<SmsOutboxWorker> logger) : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly SmsSendingOptions _opt = options.Value;
        private readonly ILogger<SmsOutboxWorker> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SmsOutboxWorker iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var outbox = scope.ServiceProvider.GetRequiredService<ISmsOutboxRepositoryPort>();
                    var sms = scope.ServiceProvider.GetRequiredService<INotificacionRepositoryPort>();

                    var items = await outbox.GetNextPendingAsync(_opt.MaxPerBatch, stoppingToken);

                    if (items.Count == 0)
                    {
                        await Task.Delay(1500, stoppingToken);
                        continue;
                    }

                    // agrupar progreso por job para actualizar en bloque
                    var progress = new Dictionary<long, (int sent, int fail)>();

                    foreach (var item in items)
                    {
                        if (stoppingToken.IsCancellationRequested) break;

                        await outbox.MarkSendingAsync(item.Id, stoppingToken);

                        if (_opt.DryRun)
                        {
                            await outbox.MarkSentAsync(item.Id, "DRYRUN", stoppingToken);
                            AddProgress(progress, item.JobId, sent: 1, fail: 0);

                            if (_opt.DelayMsBetweenSends > 0)
                                await Task.Delay(_opt.DelayMsBetweenSends, stoppingToken);

                            continue;
                        }

                        // Enviar
                        var result = await sms.SendSmsAsync(item.Phone, item.Message, stoppingToken);

                        if (result.Ok)
                        {
                            await outbox.MarkSentAsync(item.Id, result.ProviderMessageId, stoppingToken);
                            AddProgress(progress, item.JobId, sent: 1, fail: 0);
                        }
                        else
                        {
                            // Reintento vs fallo definitivo
                            var shouldRetry = item.Intentos < _opt.MaxAttempts - 1 && IsRetryable(result.ErrorMessage);

                            if (shouldRetry)
                            {
                                var next = DateTime.UtcNow.AddMinutes(BackoffMinutes(item.Intentos));
                                await outbox.MarkRetryAsync(item.Id, result.ErrorMessage ?? "Error", next, stoppingToken);
                            }
                            else
                            {
                                await outbox.MarkFailedAsync(item.Id, result.ErrorMessage ?? "Error", stoppingToken);
                                AddProgress(progress, item.JobId, sent: 0, fail: 1);
                            }
                        }

                        if (_opt.DelayMsBetweenSends > 0)
                            await Task.Delay(_opt.DelayMsBetweenSends, stoppingToken);
                    }

                    // aplicar progreso por job
                    foreach (var kv in progress)
                        await outbox.UpdateJobProgressAsync(kv.Key, kv.Value.sent, kv.Value.fail, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en SmsOutboxWorker");
                    await Task.Delay(3000, stoppingToken);
                }
            }
        }

        private static void AddProgress(Dictionary<long, (int sent, int fail)> dict, long jobId, int sent, int fail)
        {
            dict.TryGetValue(jobId, out var cur);
            dict[jobId] = (cur.sent + sent, cur.fail + fail);
        }
        private static bool IsRetryable(string? err)
        {
            if (string.IsNullOrWhiteSpace(err)) return true;

            // Heurística simple:
            // timeouts, 5xx, problemas de red => retry
            // "número inválido" => no retry
            var e = err.ToLowerInvariant();
            if (e.Contains("invalid") || e.Contains("inval") || e.Contains("número") || e.Contains("numero"))
                return false;

            return true;
        }
        private static int BackoffMinutes(int attempt)
        {
            // 1, 5, 15, 30...
            return attempt switch
            {
                0 => 1,
                1 => 5,
                2 => 15,
                _ => 30
            };
        }
    }

    public class SmsSendingOptions
    {
        public bool DryRun { get; set; } = false;
        public int MaxPerBatch { get; set; } = 50;
        public int DelayMsBetweenSends { get; set; } = 400; // ~2.5 sms/seg
        public int MaxAttempts { get; set; } = 5;
    }
}
