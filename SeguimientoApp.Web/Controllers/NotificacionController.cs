using Microsoft.AspNetCore.Mvc;
using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.UseCases.Notificacion;
using SeguimientoApp.Domain.Enums;

namespace SeguimientoApp.Web.Controllers
{
    public class NotificacionController(SendSms sendSms, ScheduleSmsBulk scheduleSmsBulk) : Controller
    {
        private readonly SendSms _sendSms = sendSms;
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
                TempData["Ok"] = $"Se programó el envío masivo. JobId: {jobId}";
                return RedirectToAction(nameof(EnviarSms));
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

        public IActionResult EnviarWhatsapp()
        {
            return View();
        }
    }
}
