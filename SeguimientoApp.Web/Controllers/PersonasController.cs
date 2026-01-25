using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.UseCases.Catalogos;
using SeguimientoApp.Application.UseCases.Personas;
using SeguimientoApp.Domain.Enums;
using SeguimientoApp.Web.Models;

namespace SeguimientoApp.Web.Controllers
{
    public class PersonasController(
        GetPersonas getPersonas,
        GetPersonaById getById,
        CreatePersona create,
        UpdatePersona update,
        ImportVotantes importarVotantes,
        GetLsCatalogo getLsCatalogo
        ) : Controller
    {
        private readonly GetPersonas _getPersonas = getPersonas;
        private readonly GetPersonaById _getById = getById;
        private readonly CreatePersona _create = create;
        private readonly UpdatePersona _update = update;
        private readonly ImportVotantes _importarVotantes = importarVotantes;
        private readonly GetLsCatalogo _getLsCatalogo = getLsCatalogo;

        public async Task<IActionResult> Index(bool? esLider, bool? estado, CancellationToken ct)
        {
            ViewBag.EsLider = esLider;
            ViewBag.Estado = estado;

            var personas = await _getPersonas.ExecuteAsync(ct);

            if (esLider.HasValue)
                personas = personas.Where(p => p.EsLider == esLider.Value).ToList();

            if (estado.HasValue)
                personas = personas.Where(p => p.Estado == estado.Value).ToList();

            return View(personas);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            await CargarTiposDocumentoAsync(ct);
            return View(new PersonaCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PersonaCreateDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await CargarTiposDocumentoAsync(ct);
                return View(model);
            }

            var idTipoDoc = model.IdTipoDocumento ?? 0;
            if (idTipoDoc > 0)
            {
                var exists = await _create.ExistsByDocumentoAsync(idTipoDoc, model.NumeroDocumento, ct);
                if (exists)
                {
                    // Se lo pegamos al NumeroDocumento para que salga debajo del input
                    ModelState.AddModelError(nameof(model.NumeroDocumento),
                        "Ya existe una persona con ese tipo y número de documento.");

                    await CargarTiposDocumentoAsync(ct);
                    return View(model);
                }
            }

            await _create.ExecuteAsync(model, ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(long id, CancellationToken ct)
        {
            var persona = await _getById.ExecuteAsync(id, ct);
            if (persona == null) return NotFound();

            return View(persona);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PersonaEditDto model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            await _update.ExecuteAsync(model, ct);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id, CancellationToken ct)
        {
            var details = await _getById.GetDetailsAsync(id, ct);
            if (details == null) return NotFound();

            return View(details);
        }

        [HttpGet]
        public IActionResult ImportarVotantes()
        {
            return View(new ImportVotantesViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarVotantes(IFormFile archivo, CancellationToken ct)
        {
            if (archivo is null || archivo.Length == 0)
            {
                TempData["Error"] = "Selecciona un archivo CSV.";
                return RedirectToAction(nameof(ImportarVotantes));
            }

            var ext = Path.GetExtension(archivo.FileName)?.ToLowerInvariant();
            if (ext != ".csv")
            {
                TempData["Error"] = "El archivo debe ser .csv";
                return RedirectToAction(nameof(ImportarVotantes));
            }

            await using var stream = archivo.OpenReadStream();
            var result = await _importarVotantes.ExecuteAsync(stream, ct);

            if (!result.Ok)
                ModelState.AddModelError("", result.Error ?? "No fue posible procesar el archivo.");

            return View(new ImportVotantesViewModel { Result = result });

        }

        [HttpGet]
        public async Task<IActionResult> LookupByCedula(long idLider, long cedula, CancellationToken ct)
        {
            var result = await _getById.LookupByCedulaAsync(idLider, cedula, ct);

            return Json(new
            {
                ok = result.Code != "NOT_FOUND",
                code = result.Code,
                persona = result.Persona,
                lider = result.LiderActual
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPersonaToLider(long idLider, long idPersona, CancellationToken ct)
        {
            var result = await _update.AddPersonaToLiderAsync(idLider, idPersona, ct);

            if (result != PersonaLiderAssignResult.Ok)
            {
                TempData["Error"] = result switch
                {
                    PersonaLiderAssignResult.SamePerson =>
                        "No puedes asignarte a ti mismo.",

                    PersonaLiderAssignResult.PersonaNotFound =>
                        "La persona no existe.",

                    PersonaLiderAssignResult.PersonaInactive =>
                        "La persona está inactiva.",

                    PersonaLiderAssignResult.PersonaIsLider =>
                        "Un líder no puede estar asignado a otro líder.",

                    PersonaLiderAssignResult.AlreadyAssigned =>
                        "La persona ya está asignada a otro líder.",

                    _ =>
                        "No fue posible asignar la persona."
                };
            }
            else
            {
                TempData["Success"] = "Persona asignada correctamente.";
            }

            return RedirectToAction(nameof(Details), new { id = idLider });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePersonaFromLider(long idLider, long idPersona, CancellationToken ct)
        {
            await _update.RemovePersonaFromLiderAsync(idLider, idPersona, ct);
            return RedirectToAction(nameof(Details), new { id = idLider });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(long id, CancellationToken ct)
        {
            await _update.ToggleEstadoAsync(id, ct);
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarTiposDocumentoAsync(CancellationToken ct)
        {
            var tiposDoc = await _getLsCatalogo.ExecuteAsync((int)TipoCatalogo.TipoDocumento, ct);
            ViewBag.TiposDocumento = tiposDoc
                .Where(x => x.Estado)
                .Select(x => new SelectListItem
                {
                    Value = x.IdCatalogo.ToString(),
                    Text = $"{x.Codigo} - {x.Nombre}"
                })
                .ToList();
        }
    }
}
