using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;
using SeguimientoApp.Domain.Enums;

namespace SeguimientoApp.Application.UseCases.Notificacion
{
    public class SendSms(INotificacionRepositoryPort repo, IPersonaRepositoryPort personaRepo)
    {
        private readonly INotificacionRepositoryPort _repo = repo;
        private readonly IPersonaRepositoryPort _personaRepo = personaRepo;

        private const int MAX_MASIVO = 2;

        public async Task<(SmsSendResultDto? single, SmsBulkResultDto? bulk, string? error)> ExecuteAsync(
            SmsModoEnvio modo,
            string message,
            string? phone,
            long? numeroDocumento,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(message))
                return (null, null, "El mensaje es obligatorio.");

            if (modo == SmsModoEnvio.NumeroManual)
            {
                var validation = ValidateAndFormatPhone(phone);

                if (!validation.ok)
                    return (null, null, validation.error);

                phone = validation.formatted; // ahora viene con 57
            }

            switch (modo)
            {
                case SmsModoEnvio.NumeroManual:
                    if (string.IsNullOrWhiteSpace(phone))
                        return (null, null, "El celular es obligatorio.");

                    var r1 = await _repo.SendSmsAsync(phone, message, ct);
                    return (r1, null, r1.Ok ? null : r1.ErrorMessage);

                case SmsModoEnvio.VotantePorDocumento:
                    if (numeroDocumento is null || numeroDocumento <= 0)
                        return (null, null, "El documento es obligatorio.");

                    var cel = await _personaRepo.GetCelularByDocumentoAsync(numeroDocumento.Value, ct);
                    if (cel is null || cel <= 0)
                        return (null, null, "El votante no tiene celular registrado.");

                    var validationDoc = ValidateAndFormatPhone(cel.Value.ToString());

                    if (!validationDoc.ok)
                        return (null, null, $"Celular inválido en base de datos: {validationDoc.error}");

                    var r2 = await _repo.SendSmsAsync(validationDoc.formatted!, message, ct);
                    return (r2, null, r2.Ok ? null : r2.ErrorMessage);

                case SmsModoEnvio.MasivoVotantesActivosNoLideres:
                    var celulares = await _personaRepo.GetCelularesVotantesActivosNoLideresAsync(ct);
                    var lista = celulares.Take(MAX_MASIVO).ToList();

                    var errores = new List<string>();
                    int ok = 0, fail = 0;

                    foreach (var c in lista)
                    {
                        var val = ValidateAndFormatPhone(c.ToString());

                        if (!val.ok)
                        {
                            fail++;
                            errores.Add($"{c}: {val.error}");
                            continue;
                        }

                        var rr = await _repo.SendSmsAsync(val.formatted!, message, ct);
                        if (rr.Ok) ok++;
                        else
                        {
                            fail++;
                            errores.Add($"{c}: {rr.ErrorMessage}");
                        }
                    }

                    var result = new SmsBulkResultDto
                    {
                        Total = lista.Count,
                        Ok = ok,
                        Fail = fail,
                        Errores = errores
                    };

                    return (null, result, null);

                default:
                    return (null, null, "Modo inválido.");
            }
        }

        private (bool ok, string? formatted, string? error) ValidateAndFormatPhone(string? rawPhone)
        {
            if (string.IsNullOrWhiteSpace(rawPhone))
                return (false, null, "El celular es obligatorio.");

            var phone = rawPhone.Trim();

            // Debe ser solo números
            if (!phone.All(char.IsDigit))
                return (false, null, "El celular solo debe contener números.");

            // Debe tener exactamente 10 caracteres
            if (phone.Length != 10)
                return (false, null, "El celular debe tener exactamente 10 dígitos.");

            // Debe empezar por 3 (celulares en Colombia)
            if (!phone.StartsWith("3"))
                return (false, null, "El celular debe iniciar por 3.");

            // Agregamos prefijo país
            var formatted = $"57{phone}";

            return (true, formatted, null);
        }

    }
}
