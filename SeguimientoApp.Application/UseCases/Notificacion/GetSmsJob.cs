using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Application.UseCases.Notificacion
{
    public sealed class GetSmsJob(ISmsOutboxRepositoryPort repo)
    {
        private readonly ISmsOutboxRepositoryPort _repo = repo;
        public Task<SmsJobStatusDto?> ExecuteAsync(long jobId, CancellationToken ct = default)
            => _repo.GetJobStatusAsync(jobId, ct);

        public Task<List<SmsJobListItemDto>> GetRecentJobsAsync(int take = 20, CancellationToken ct = default)
        => _repo.GetRecentJobsAsync(take, ct);
    }
}
