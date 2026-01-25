namespace SeguimientoApp.Application.DTOs
{
    public class PersonaDocumentoLookupDto
    {
        public long IdPersona { get; set; }
        public int IdTipoDocumento { get; set; }
        public long NumeroDocumento { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public bool EsLider { get; set; }
        public bool Estado { get; set; }
    }
}
