using System.ComponentModel.DataAnnotations;

namespace SeguimientoApp.Application.DTOs
{
    public class EventoCreateDto
    {
        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(150)]
        public string Nombre { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona el tipo de evento.")]
        public int IdTipoEventoCat { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona el estado del evento.")]
        public int IdEstadoEventoCat { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        [StringLength(150)]
        public string? Lugar { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
