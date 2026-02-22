using Microsoft.EntityFrameworkCore;
using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;
using SeguimientoApp.Infrastructure.Persistence.MySql.Models;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Repositories
{
    public class SmsOutboxRepository(AppDbContext db) : ISmsOutboxRepositoryPort
    {
        private readonly AppDbContext _db = db;

        public async Task<long> CreateJobAsync(string message, string target, int total, CancellationToken ct = default)
        {
            var job = new SmsJobModel
            {
                Message = message,
                Target = target,
                Total = total,
                Status = "QUEUED",
                CreatedAt = DateTime.UtcNow
            };

            _db.SmsJobModels.Add(job);
            await _db.SaveChangesAsync(ct);
            return job.IdJob;
        }

        public async Task EnqueueAsync(long jobId, IEnumerable<string> phones, CancellationToken ct = default)
        {
            var list = phones
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct()
                .ToList();

            if (list.Count == 0) return;

            var now = DateTime.UtcNow;

            var items = list.Select(p => new SmsOutboxModel
            {
                IdJob = jobId,
                Phone = p,
                Estado = "PENDING",
                Intentos = 0,
                NextAttemptAt = now,
                CreatedAt = now
            }).ToList();

            _db.SmsOutboxModels.AddRange(items);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<SmsOutboxItemDto>> GetNextPendingAsync(int take, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            return await _db.SmsOutboxModels
                .AsNoTracking()
                .Include(x => x.Job)
                .Where(x => (x.Estado == "PENDING" || x.Estado == "RETRY") && x.NextAttemptAt <= now)
                .OrderBy(x => x.NextAttemptAt)
                .ThenBy(x => x.Id)
                .Take(take)
                .Select(x => new SmsOutboxItemDto()
                {
                    Id = x.Id,
                    JobId = x.IdJob,
                    Phone = x.Phone,
                    Intentos = x.Intentos,
                    Message = x.Job.Message,
                })
                .ToListAsync(ct);
        }

        public async Task MarkSendingAsync(long outboxId, CancellationToken ct = default)
        {
            var item = await _db.SmsOutboxModels.FirstOrDefaultAsync(x => x.Id == outboxId, ct);
            if (item == null) return;

            item.Estado = "SENDING";
            item.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        public async Task MarkSentAsync(long outboxId, string? providerMessageId, CancellationToken ct = default)
        {
            var item = await _db.SmsOutboxModels.FirstOrDefaultAsync(x => x.Id == outboxId, ct);
            if (item == null) return;

            item.Estado = "SENT";
            item.ProviderMessageId = providerMessageId;
            item.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        public async Task MarkRetryAsync(long outboxId, string lastError, DateTime nextAttemptAt, CancellationToken ct = default)
        {
            var item = await _db.SmsOutboxModels.FirstOrDefaultAsync(x => x.Id == outboxId, ct);
            if (item == null) return;

            item.Estado = "RETRY";
            item.Intentos += 1;
            item.LastError = lastError.Length > 500 ? lastError[..500] : lastError;
            item.NextAttemptAt = nextAttemptAt;
            item.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        public async Task MarkFailedAsync(long outboxId, string lastError, CancellationToken ct = default)
        {
            var item = await _db.SmsOutboxModels.FirstOrDefaultAsync(x => x.Id == outboxId, ct);
            if (item == null) return;

            item.Estado = "FAILED";
            item.Intentos += 1;
            item.LastError = lastError.Length > 500 ? lastError[..500] : lastError;
            item.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateJobProgressAsync(long jobId, int sentDelta, int failedDelta, CancellationToken ct = default)
        {
            var job = await _db.SmsJobModels.FirstOrDefaultAsync(x => x.IdJob == jobId, ct);
            if (job == null) return;

            job.Sent += sentDelta;
            job.Failed += failedDelta;

            if (job.Sent + job.Failed >= job.Total)
                job.Status = "DONE";
            else if (job.Status == "QUEUED")
                job.Status = "RUNNING";

            await _db.SaveChangesAsync(ct);
        }
    }
}
