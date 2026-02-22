using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Application.UseCases.Notificacion
{
    public class ScheduleSmsBulk(IPersonaRepositoryPort personaRepo, ISmsOutboxRepositoryPort outboxRepo)
    {
        private readonly IPersonaRepositoryPort _personaRepo = personaRepo;
        private readonly ISmsOutboxRepositoryPort _outboxRepo = outboxRepo;

        public async Task<long> ExecuteAsync(string message, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("El mensaje es obligatorio", nameof(message));

            var celulares = await _personaRepo.GetCelularesVotantesActivosNoLideresAsync(ct);

            var phones = celulares
                .Select(x => x.ToString())
                .Select(x => TryFormatColPhone(x))
                .Where(x => x.ok && x.formatted != null)
                .Select(x => x.formatted!)
                .Distinct()
                .ToList();

            var jobId = await _outboxRepo.CreateJobAsync(message.Trim(), "VotantesActivosNoLideres", phones.Count, ct);
            await _outboxRepo.EnqueueAsync(jobId, phones, ct);

            return jobId;
        }

        private static (bool ok, string? formatted) TryFormatColPhone(string raw10)
        {
            if (raw10.Length != 10) return (false, null);
            if (!raw10.StartsWith("3")) return (false, null);
            if (!raw10.All(char.IsDigit)) return (false, null);
            return (true, "57" + raw10);
        }
    }
}
