using Microsoft.AspNetCore.Mvc;
using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.UseCases.Notificacion;

namespace SeguimientoApp.Web.Controllers
{
    public class NotificacionController(SendSms sendSms) : Controller
    {
        private readonly SendSms _sendSms = sendSms;

        public IActionResult EnviarSms()
        {
            var model = new EnviarSmsDto();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EnviarSms(EnviarSmsDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var r = await _sendSms.ExecuteAsync(model.Phone, model.Message, ct);

            if (!r.Ok)
            {
                ViewBag.Error = r.ErrorMessage;
                return View();
            }

            ViewBag.Ok = $"Enviado ✅ | ProviderId={r.ProviderMessageId} | State={r.ProviderState}";
            return RedirectToAction(nameof(EnviarSms));
        }

        public IActionResult EnviarWhatsapp()
        {
            return View();
        }
    }
}
