using SeguimientoApp.Application.DTOs;

namespace SeguimientoApp.Application.Ports.Persistence
{
    public interface ISmsOutboxRepositoryPort
    {
        Task<SmsJobStatusDto?> GetJobStatusAsync(long jobId, CancellationToken ct = default);
        Task<long> CreateJobAsync(string message, string target, int total, CancellationToken ct = default);
        Task EnqueueAsync(long jobId, IEnumerable<string> phones, CancellationToken ct = default);
        Task<List<SmsJobListItemDto>> GetRecentJobsAsync(int take, CancellationToken ct = default);

        Task<List<SmsOutboxItemDto>> GetNextPendingAsync(int take, CancellationToken ct = default);

        Task MarkSendingAsync(long outboxId, CancellationToken ct = default);
        Task MarkSentAsync(long outboxId, string? providerMessageId, CancellationToken ct = default);
        Task MarkRetryAsync(long outboxId, string lastError, DateTime nextAttemptAt, CancellationToken ct = default);
        Task MarkFailedAsync(long outboxId, string lastError, CancellationToken ct = default);
        Task UpdateJobProgressAsync(long jobId, int sentDelta, int failedDelta, CancellationToken ct = default);
    }
}
