using Microsoft.AspNetCore.Mvc;
using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.UseCases.Notificacion;
using SeguimientoApp.Domain.Enums;

namespace SeguimientoApp.Web.Controllers
{
    public class NotificacionController(SendSms sendSms, GetSmsJob getSms, ScheduleSmsBulk scheduleSmsBulk) : Controller
    {
        private readonly SendSms _sendSms = sendSms;
        private readonly GetSmsJob _getSms = getSms;
        private readonly ScheduleSmsBulk _scheduleSmsBulk = scheduleSmsBulk;

        public IActionResult EnviarSms()
        {
            var model = new EnviarSmsDto() { Modo = (int)SmsModoEnvio.MasivoVotantesActivosNoLideres };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EnviarSms(EnviarSmsDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(model);

            var modo = (SmsModoEnvio)model.Modo;

            if (modo == SmsModoEnvio.MasivoVotantesActivosNoLideres)
            {
                var jobId = await _scheduleSmsBulk.ExecuteAsync(model.Message, ct);

                HttpContext.Session.SetString("LastSmsJobId", jobId.ToString());
                return RedirectToAction(nameof(EstadoJob), new { id = jobId });
            }

            var (single, bulk, error) = await _sendSms.ExecuteAsync(modo, model.Message, model.Phone, model.NumeroDocumento, ct);

            if (error != null)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(EnviarSms));
            }

            if (single != null)
            {
                TempData["Ok"] = $"Enviado ✅ ProviderId={single.ProviderMessageId} State={single.ProviderState}";
                return RedirectToAction(nameof(EnviarSms));
            }

            if (bulk != null)
            {
                TempData["Ok"] = $"Masivo ✅ Total={bulk.Total} OK={bulk.Ok} Fallidos={bulk.Fail}";
                if (bulk.Errores.Count > 0)
                    TempData["Warn"] = string.Join("\n", bulk.Errores);
                return RedirectToAction(nameof(EnviarSms));
            }

            TempData["Error"] = "No hubo resultado.";
            return RedirectToAction(nameof(EnviarSms));
        }

        [HttpGet]
        public async Task<IActionResult> EstadoJob(long id, CancellationToken ct)
        {
            var status = await _getSms.ExecuteAsync(id, ct);
            if (status == null)
            {
                TempData["Error"] = $"No existe el Job {id}";
                return RedirectToAction(nameof(EnviarSms));
            }

            return View(status);
        }

        public IActionResult EnviarWhatsapp()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Jobs(CancellationToken ct)
        {
            var items = await _getSms.GetRecentJobsAsync(30, ct);
            return View(items);
        }

        [HttpGet]
        public IActionResult UltimoJob()
        {
            var last = HttpContext.Session.GetString("LastSmsJobId");
            if (string.IsNullOrWhiteSpace(last) || !long.TryParse(last, out var jobId))
            {
                TempData["Error"] = "No hay un Job reciente para mostrar.";
                return RedirectToAction(nameof(EnviarSms));
            }

            return RedirectToAction(nameof(EstadoJob), new { id = jobId });
        }
    }
}
