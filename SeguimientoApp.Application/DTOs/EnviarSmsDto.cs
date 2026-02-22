using System.ComponentModel.DataAnnotations;

namespace SeguimientoApp.Application.DTOs
{
    public class EnviarSmsDto
    {
        public int Modo { get; set; } = 3;

        [Display(Name = "Celular")]
        [RegularExpression(@"^\d{10,15}$", ErrorMessage = "Ingrese un número válido (10 a 15 dígitos).")]
        public string? Phone { get; set; }

        [Display(Name = "Documento del votante")]
        [Range(1, long.MaxValue, ErrorMessage = "Documento inválido.")]
        public long? NumeroDocumento { get; set; }

        [Required(ErrorMessage = "El mensaje es obligatorio.")]
        [StringLength(160, ErrorMessage = "El mensaje no puede superar los 160 caracteres.")]
        public string Message { get; set; } = string.Empty;
    }
}
