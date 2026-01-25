namespace SeguimientoApp.Application.DTOs
{
    public class EventoDto
    {
        public int IdEvento { get; set; }
        public string Nombre { get; set; } = "";

        public int IdTipoEventoCat { get; set; }
        public string TipoEventoNombre { get; set; } = "";
        public string TipoEventoCodigo { get; set; } = "";

        public int IdEstadoEventoCat { get; set; }
        public string EstadoEventoNombre { get; set; } = "";
        public string EstadoEventoCodigo { get; set; } = "";

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public string Lugar { get; set; } = "";
        public string Observaciones { get; set; } = "";
    }
}
