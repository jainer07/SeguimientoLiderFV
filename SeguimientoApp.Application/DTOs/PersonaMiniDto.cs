namespace SeguimientoApp.Application.DTOs
{
    public class PersonaMiniDto
    {
        public long IdPersona { get; set; }
        public string TipoDocumentoCodigo { get; set; } = "";
        public long NumeroDocumento { get; set; }
        public string NombreCompleto { get; set; } = "";
        public long? Celular { get; set; }
        public bool Estado { get; set; }
    }
}
