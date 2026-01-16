using System.ComponentModel.DataAnnotations;

namespace SeguimientoApp.Web.Models
{
    public class PersonaEditViewModel
    {
        public long IdPersona { get; set; }

        [Required]
        public int IdTipoDocumento { get; set; }

        [Required]
        public long NumeroDocumento { get; set; }

        public bool EsLider { get; set; }
        public bool Estado { get; set; }

        [Required] public string PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        [Required] public string PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }

        public long Celular { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
        public string? LugarVotacion { get; set; }
        public string? Mesa { get; set; }
    }
}
