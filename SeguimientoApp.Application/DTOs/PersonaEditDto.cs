using System.ComponentModel.DataAnnotations;

namespace SeguimientoApp.Application.DTOs
{
    public class PersonaEditDto
    {
        public long IdPersona { get; set; }
        public string TipoDocumentoCodigo { get; set; } = "";
        public long NumeroDocumento { get; set; }

        [Required(ErrorMessage = "El primer nombre es obligatorio")]
        [StringLength(60, ErrorMessage = "Máximo 60 caracteres")]
        public string PrimerNombre { get; set; } = "";

        [StringLength(60, ErrorMessage = "Máximo 60 caracteres")]
        public string? SegundoNombre { get; set; } = "";

        [Required(ErrorMessage = "El primer apellido es obligatorio")]
        [StringLength(60, ErrorMessage = "Máximo 60 caracteres")]
        public string PrimerApellido { get; set; } = "";

        [StringLength(60, ErrorMessage = "Máximo 60 caracteres")]
        public string? SegundoApellido { get; set; } = "";

        [Required(ErrorMessage = "El celular es obligatorio")]
        [Range(1000000000, 3999999999, ErrorMessage = "Celular inválido")]
        public long? Celular { get; set; }

        [EmailAddress(ErrorMessage = "Correo inválido")]
        [StringLength(120, ErrorMessage = "Máximo 120 caracteres")]
        public string? Correo { get; set; } = "";

        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        public string? Direccion { get; set; } = "";

        [Range(1, 9999, ErrorMessage = "Mesa inválida")]
        public int? MesaNumero { get; set; }

        public string? LugarVotacion { get; set; } = "";
        public string? Mesa { get; set; } = "";
        public bool EsLider { get; set; }
        public bool Estado { get; set; }
    }
}
