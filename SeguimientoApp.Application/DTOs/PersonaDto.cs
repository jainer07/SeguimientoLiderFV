namespace SeguimientoApp.Application.DTOs
{
    public class PersonaDto
    {
        public long IdPersona { get; set; }
        public string TipoDocumentoCodigo { get; set; } = "";
        public long NumeroDocumento { get; set; }
        public bool EsLider { get; set; }
        public string PrimerNombre { get; set; } = "";
        public string SegundoNombre { get; set; } = "";
        public string PrimerApellido { get; set; } = "";
        public string SegundoApellido { get; set; } = "";
        public long Celular { get; set; }
        public string Correo { get; set; } = "";
        public string Direccion { get; set; } = "";
        public bool Estado { get; set; }
        public string LugarVotacion { get; set; } = "";
        public string Mesa { get; set; } = "";
    }
}
