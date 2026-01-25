using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.UseCases.Catalogos;
using SeguimientoApp.Application.UseCases.Eventos;
using SeguimientoApp.Domain.Enums;

namespace SeguimientoApp.Web.Controllers
{
    public class EventosController(
        GetEventos getEventos,
        GetEventoById getById,
        CreateEvento create,
        UpdateEvento update,
        GetLsCatalogo getLsCatalogo
    ) : Controller
    {
        private readonly GetEventos _getEventos = getEventos;
        private readonly GetEventoById _getById = getById;
        private readonly CreateEvento _create = create;
        private readonly UpdateEvento _update = update;
        private readonly GetLsCatalogo _getLsCatalogo = getLsCatalogo;

        public async Task<IActionResult> Index(int? tipoEvento, int? estadoEvento, CancellationToken ct)
        {
            ViewBag.TipoEvento = tipoEvento;
            ViewBag.EstadoEvento = estadoEvento;

            var eventos = await _getEventos.ExecuteAsync(ct);

            if (tipoEvento.HasValue)
                eventos = eventos.Where(x => x.IdTipoEventoCat == tipoEvento.Value).ToList();

            if (estadoEvento.HasValue)
                eventos = eventos.Where(x => x.IdEstadoEventoCat == estadoEvento.Value).ToList();

            return View(eventos);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await CargarCatalogosAsync(ct);
            return View(new EventoCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventoCreateDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(ct);
                return View(model);
            }

            await _create.ExecuteAsync(model, ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var evento = await _getById.ExecuteAsync(id, ct);
            if (evento == null) return NotFound();

            await CargarCatalogosAsync(ct);
            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventoEditDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync(ct);
                return View(model);
            }

            await _update.ExecuteAsync(model, ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var evento = await _getById.ExecuteAsync(id, ct);
            if (evento == null) return NotFound();

            await CargarCatalogosAsync(ct);
            return View(evento);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEstado(int idEvento, int idEstadoEventoCat, CancellationToken ct)
        {
            if (idEvento <= 0 || idEstadoEventoCat <= 0)
            {
                TempData["Error"] = "Datos inválidos para cambiar el estado del evento.";
                return RedirectToAction(nameof(Index));
            }

            await _update.UpdateEstadoAsync(idEvento, idEstadoEventoCat, ct);
            TempData["Success"] = "Estado actualizado.";
            return RedirectToAction(nameof(Details), new { id = idEvento });
        }

        private async Task CargarCatalogosAsync(CancellationToken ct)
        {
            var tiposEvento = await _getLsCatalogo.ExecuteAsync((int)TipoCatalogo.TipoEvento, ct);
            ViewBag.TiposEvento = tiposEvento
                .Where(x => x.Estado)
                .Select(x => new SelectListItem
                {
                    Value = x.IdCatalogo.ToString(),
                    Text = x.Nombre
                })
                .ToList();

            var estadosEvento = await _getLsCatalogo.ExecuteAsync((int)TipoCatalogo.EstadoEvento, ct);
            ViewBag.EstadosEvento = estadosEvento
                .Where(x => x.Estado)
                .Select(x => new SelectListItem
                {
                    Value = x.IdCatalogo.ToString(),
                    Text = x.Nombre
                })
                .ToList();
        }
    }
}
